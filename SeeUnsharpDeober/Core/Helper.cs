using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeUnsharpDeober.Core
{
    class Helper
    {
        public static ModuleDefMD asm;

        public static MethodDef GetDecryptType(ModuleDefMD asmodule)
        {
            //
            var module_types = asmodule.Types;
            asm = asmodule;
            //
            foreach (TypeDef type in module_types)
            {
                if (type.Name == "<Module>") continue;
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    if (!method.IsConstructor) continue;
                    var instrsdec = method.Body.Instructions;
                    if (instrsdec.Count < 5) continue;
                    for (int i = 0; i < instrsdec.Count; i++)
                    {
                        if (method.Body.Instructions[0].OpCode == OpCodes.Ldstr && method.Body.Instructions[1].OpCode == OpCodes.Call 
                            && method.Body.Instructions[2].OpCode == OpCodes.Stsfld)
                        {
                           
                            return method;
                        }
                      
                    }
                }
            }
            return null;
        }

        public static int Extract_string_value(MethodDef method_)
        {
            //
            var module_types = asm.Types;
            var count = 0;
            //
            var instrsdec = method_.Body.Instructions;
            for (int i = 0; i < instrsdec.Count; i++)
            {
                if (method_.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                {
                    string str_to_decrypt = (string)method_.Body.Instructions[i].Operand;
                    string decrypted_obj = decrypt_strings(str_to_decrypt);
                    if (method_.Body.Instructions[i + 2].OpCode == OpCodes.Stsfld)
                    {
                        var my_field = (FieldDef)method_.Body.Instructions[i + 2].Operand;
                        if(ReplaceString(decrypted_obj, my_field))
                        {
                            Console.WriteLine("[+] " + decrypted_obj + " : " + my_field.Name);
                            count++;
                        }
                    }
                }

            }
            return count;
        }


        public static bool ReplaceString(string decrypted_string, FieldDef field_)
        {
            //
            var module_types = asm.Types;
            //
            foreach (TypeDef type in module_types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    var instrsdec = method.Body.Instructions;
                    for (int i = 0; i < instrsdec.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldsfld && method.Body.Instructions[i].Operand != null)
                        {
                            try
                            {
                                var got_field = (FieldDef)method.Body.Instructions[i].Operand;
                                if (got_field.MDToken == field_.MDToken)
                                {
                                    method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                                    method.Body.Instructions[i].Operand = decrypted_string;
                                    return true;
                                }
                            }
                            catch (Exception)
                            {

                                //
                            }
                                                   
                        }

                    }
                }
            }
            return false;
        }

        public static string decrypt_strings(string str)
        {
	        char[] array = str.ToCharArray();
                byte[] array2 = new byte[array.Length * 2];
	        for (int i = 0; i<array.Length; i++)
	        {
		        array2[i * 2] = (byte)(array[i] >> 8);
		        array2[i * 2 + 1] = (byte)(array[i] & 'ÿ');
	        }
            Array.Reverse(array2);
	        int num = array2.Length - 2;
	        if (array2[0] == 1)
	        {
		        num++;
	        }
            byte[] array3 = new byte[num];
	        for (int j = 0; j<array3.Length; j++)
	        {
		        array3[j] = (byte)((int)array2[j + 1] - array3.Length* 123 - j* 41);
	        }
	        return Encoding.UTF8.GetString(array3);
        }

    }
}
