﻿using System;
using dnlib.DotNet;
using kov.NET.Utils;
using kov.NET.Utils.Analyzer;

namespace kov.NET.Protections
{
    public class Renamer : Randomizer
    {
        private static int MethodAmount { get; set; }

        private static int ParameterAmount { get; set; }

        private static int PropertyAmount { get; set; }

        private static int FieldAmount { get; set; }

        private static int EventAmount { get; set; }

        /// <summary>
        /// Execution of the 'Renamer' method. It'll rename types, methods and their parameters, properties, fields and events to random strings. But before they get renamed, they get analyzed to see if they are good to be renamed. (That prevents the program being broken)
        /// </summary>
        public static void Execute()
        {
            if (Program.DontRename) return;

            Program.Module.Mvid = Guid.NewGuid();
            Program.Module.Name = GenerateRandomString(MemberRenamer.StringLength());
            Program.Module.EntryPoint.Name = GenerateRandomString(MemberRenamer.StringLength());

            foreach (TypeDef type in Program.Module.Types)
            {
                foreach (MethodDef m in type.Methods)
                {
                    if (CanRename(m) && !Program.ForceWinForms && !Program.FileExtension.Contains("dll"))
                    {
                        m.Name = GenerateRandomString(MemberRenamer.StringLength());
                        ++MethodAmount;
                    }

                    foreach (Parameter para in m.Parameters)
                        if (CanRename(para))
                        {
                            para.Name = GenerateRandomString(MemberRenamer.StringLength());
                            ++ParameterAmount;
                        }
                }

                foreach (PropertyDef p in type.Properties)
                    if (CanRename(p))
                    {
                        p.Name = GenerateRandomString(MemberRenamer.StringLength());
                        ++PropertyAmount;
                    }

                foreach (FieldDef field in type.Fields)
                    if (CanRename(field))
                    {
                        field.Name = GenerateRandomString(MemberRenamer.StringLength());
                        ++FieldAmount;
                    }

                foreach (EventDef e in type.Events)
                    if (CanRename(e))
                    {
                        e.Name = GenerateRandomString(MemberRenamer.StringLength());
                        ++EventAmount;
                    }
            }

            Console.WriteLine($"  Renamed {MethodAmount} methods.\n  Renamed {ParameterAmount} parameters." +
                $"\n  Renamed {PropertyAmount} properties.\n  Renamed {FieldAmount} fields.\n  Renamed {EventAmount} events.");
        }

        /// <summary>
        /// This will check with some analyzers if it's possible to rename a member def { TypeDef, PropertyDef, MethodDef, EventDef, FieldDef, Parameter (NOT DEF) }.
        /// </summary>
        /// <param name="obj">The determinate to check.</param>
        /// <returns>If the determinate can be renamed.</returns>
		public static bool CanRename(object obj)
        {
            DefAnalyzer analyze;
            if (obj is MethodDef) analyze = new MethodDefAnalyzer();
            else if (obj is PropertyDef) analyze = new PropertyDefAnalyzer();
            else if (obj is EventDef) analyze = new EventDefAnalyzer();
            else if (obj is FieldDef) analyze = new FieldDefAnalyzer();
            else if (obj is Parameter) analyze = new ParameterAnalyzer();
            else return false;
            return analyze.Execute(obj);
        }
    }
}
