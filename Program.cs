using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using kov.NET.Protections;
using kov.NET.Utils;
using IniParser;
using IniParser.Model;
using System.Diagnostics;

namespace kov.NET
{
    class Program
    {
        public static ModuleDefMD Module { get; set; }
        public ModuleDef ManifestModule;

        public static string FileExtension { get; set; }

        public static bool DontRename { get; set; }

        public static bool ForceWinForms { get; set; }

        public static string FilePath { get; set; }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Process[] pname = Process.GetProcessesByName("dnSpy");
            foreach (Process proc in pname)
            {
                //bad process check
                Console.WriteLine($"You Are Using The Forbidden Process {proc.ProcessName}, Please Close This Application Before Running This Program Again.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            string path;
            if (args.Length == 0)
            {
                Console.WriteLine("Drag n drop your file : ");
                path = Console.ReadLine();
            }
            else
            {
                path = args[0];
            }
            Module = ModuleDefMD.Load(path);
            FileExtension = Path.GetExtension(path);


            bool StringEncrypt = false;
            bool Renaming = false;
            bool IntEncode = false;
            bool ContorFlow = false;
            bool LocalToFields = false;
            bool ProxyInt = false;
            bool AntiD4Dot = false;

            if (!File.Exists(@"settings.ini"))
            {
                IniData iniData = new IniData();
                iniData["ObfuscationSettings"]["StringEncryption"] = "true"; // or "false" based on your logic
                iniData["ObfuscationSettings"]["Renaming"] = "true"; // or "false" based on your logic
                iniData["ObfuscationSettings"]["IntEncoding"] = "true"; // or "false" based on your logic
                iniData["ObfuscationSettings"]["ControlFlow"] = "true"; // or "false" based on your logic
                iniData["ObfuscationSettings"]["LocalToFields"] = "false"; // or "false" based on your logic
                iniData["ObfuscationSettings"]["ProxyInts"] = "true"; // or "false" based on your logic
                iniData["ObfuscationSettings"]["AntiDe4Dot"] = "true"; // or "false" based on your logic

                FileIniDataParser parser = new FileIniDataParser();
                parser.WriteFile("settings.ini", iniData);
            }
            else
            {
                FileIniDataParser parser = new FileIniDataParser();
                IniData iniData = parser.ReadFile("settings.ini");

                string isEnabledStr = iniData["ObfuscationSettings"]["StringEncryption"];
                string isEnabledStr1 = iniData["ObfuscationSettings"]["Renaming"];
                string isEnabledStr2 = iniData["ObfuscationSettings"]["IntEncoding"];
                string isEnabledStr3 = iniData["ObfuscationSettings"]["ControlFlow"];
                string isEnabledStr4 = iniData["ObfuscationSettings"]["LocalToFields"];
                string isEnabledStr5 = iniData["ObfuscationSettings"]["ProxyInts"];
                string isEnabledStr6 = iniData["ObfuscationSettings"]["AntiDe4Dot"];
                StringEncrypt = bool.Parse(isEnabledStr);
                Renaming = bool.Parse(isEnabledStr1);
                IntEncode = bool.Parse(isEnabledStr2);
                ContorFlow = bool.Parse(isEnabledStr3);
                LocalToFields = bool.Parse(isEnabledStr4);
                ProxyInt = bool.Parse(isEnabledStr5);
                AntiD4Dot = bool.Parse(isEnabledStr6);
            }


            if (StringEncrypt == true)
            {
                Console.WriteLine("Encrypting strings...");
                StringEncryption.Execute();
            }



            if (Renaming == true)
            {
                Console.WriteLine("Renaming...");
                Renamer.Execute();
            }


            if (IntEncode == true)
            {
                Console.WriteLine("Encoding ints...");
                IntEncoding.Execute();
            }

            if (ContorFlow == true)
            {
                Console.WriteLine("Injecting ControlFlow...");
                ControlFlow.Execute();
            }



            if (LocalToFields == true)
            {
                Console.WriteLine("Injecting local to fields...");
                L2F.Execute();
            }


            if (ProxyInt == true)
            {
                Console.WriteLine("Adding Proxys...");
                ProxyInts.Execute();
            }

            if (AntiD4Dot == true)
            {
                Console.WriteLine("Injecting AntiDe4Dot...");
                AntiDe4Dot.Execute();
            }







            Console.WriteLine("Saving file...");
            var pathez = $"{path}-kiv.exe";
            ModuleWriterOptions opts = new ModuleWriterOptions(Module) { Logger = DummyLogger.NoThrowInstance };
            Module.Write(pathez, opts);


            Console.WriteLine("Done! Press any key to exit...");
            Console.ReadKey();
        }
    }
}