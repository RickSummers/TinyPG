using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TinyPG
{
    public class Program48
    {
        public enum ExitCode : int
        {
            Success = 0,
            InvalidFilename = 1,
            UnknownError = 10
        }

        [STAThread]
        public static int Main(string[] args)
        {
            return Program.Main(args);
        }
    }

    internal static class Program
    {
        public enum ExitCode : int
        {
            Success = 0,
            InvalidFilename = 1,
            UnknownError = 10
        }

        [STAThread]
        public static int Main(string[] args)
        {
            var sources = new List<string>();
            string assemblyFileName = null;
            string errorFileName = null;
            var nullableContext = true;
            string language = null;
            var references = new List<string>();

            var i = 0;
            while (i < args.Length)
            {
                var a = args[i].Trim();

                if (a == "-a")
                {
                    assemblyFileName = args[++i].Trim();
                }
                else if (a == "-e")
                {
                    errorFileName = args[++i].Trim();
                }
                else if ((a == "-n") || (a == "-n+"))
                {
                    nullableContext = true;
                }
                else if (a == "-n-")
                {
                    nullableContext = false;
                }
                else if ((a == "-c") || (a == "-c#"))
                {
                    language = "C#";
                }
                else if ((a == "-b") || (a == "-vb"))
                {
                    language = "VB";
                }
                else if (a == "-r")
                {
                    references.Add(args[++i].Trim());
                }
                else
                {
                    sources.Add(args[i].Trim());
                }
                ++i;
            }

            var helper = new TinyPG.CompileHelper();
            helper.Language = language ?? "C#";
            helper.NullableContext = nullableContext;
            foreach (var source in sources )
                helper.Sources.Add(File.ReadAllText(source.TrimEnd()));
            foreach (var reference in references)
                helper.References.Add(reference);
            helper.AssemblyFileName = assemblyFileName ?? Path.GetTempFileName();
            
            errorFileName = errorFileName ?? Path.GetTempFileName();
            try
            {
                helper.BuildCode();
            }
            catch (Exception)
            {
                var errors = helper.Errors;
                File.WriteAllLines(errorFileName, errors);
            }

            return (int) ExitCode.Success;
        }


    }
}
