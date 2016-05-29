using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeUnsharpDeober
{
    class Program
    {
        static ModuleDefMD asm;
        public static string asmpath;
        public static int DeobedStringNumber;

        static void Main(string[] args)
        {

            Console.WriteLine(@" _____     _____ _                       ");
            Console.WriteLine(@"|  |  |___|   __| |_ ___ ___ ___ ___ ___ ");
            Console.WriteLine(@"|  |  |   |__   |   | .'|  _| . | -_|  _|");
            Console.WriteLine(@"|_____|_|_|_____|_|_|__,|_| |  _|___|_|  ");
            Console.WriteLine(@"                            |_|XenocodeRCE");
            Console.WriteLine(@"");
            Console.WriteLine(@"");
            if (args == null || args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!]Error : No file to deobfuscate ! ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                return;
            }
            else{
                try
                {
                    asm = ModuleDefMD.Load(args[0]);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("[!]Loading assembly " + asm.FullName);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    asmpath = args[0];
                    var dec_method = Core.Helper.GetDecryptType(asm);
                    if(dec_method != null)
                    {
                        Console.WriteLine("[!]Instancing decryption method : " + dec_method.FullName);
                        Console.WriteLine("[!]Decrypting Strings ... : ");
                        var decryptedstr = Core.Helper.Extract_string_value(dec_method);
                        if(decryptedstr != 0)
                        {
                            DeobedStringNumber = decryptedstr;
                        }

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(@"[!] Successfully decrypted " + DeobedStringNumber + " strings.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(@"[!] Saving Module...");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        string text2 = Path.GetDirectoryName(args[0]);
                        if (!text2.EndsWith("\\"))
                        {
                            text2 += "\\";
                        }
                        string path = text2 + Path.GetFileNameWithoutExtension(args[0]) + "_patched" +
                                      Path.GetExtension(args[0]);
                        var opts = new ModuleWriterOptions(asm);
                        opts.Logger = DummyLogger.NoThrowInstance;
                       
                        asm.Write(path, opts);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(@"[!] Saved ! ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[!]Error : Cannot find the decryption method !");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.ReadKey();
                        return;
                    }
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[!]Error : Cannot load the file. Make sure it's a valid .NET file !");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    return;
                }
            }
        }
    }
}
