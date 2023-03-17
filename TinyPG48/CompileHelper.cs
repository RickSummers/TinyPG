using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeDom = System.CodeDom.Compiler;

namespace TinyPG
{
    public class CompileHelper
    {
        public string Language { get; set; } = "C#";
        public bool NullableContext { get; set; } = true;
        public readonly List<string> Sources = new List<string>();
        public readonly List<string> References = new List<string>();
        public readonly List<string> Errors = new List<string>();
        public Assembly CompiledAssembly { get; private set; } = null;
        public ICodeGenerator ParserGenerator { get; set; }
        public ICodeGenerator ScannerGenerator { get; set; }
        public ICodeGenerator ParseTreeGenerator { get; set; }
        public ICodeGenerator HighlighterGenerator { get; set; }
        public string AssemblyFileName { get; set; }

        public Assembly BuildCode()
        {
            Errors.Clear();
            CompiledAssembly = null;

            CompilerResults Result;
            CodeDomProvider provider = CodeGeneratorFactory.CreateCodeDomProvider(Language);
            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateInMemory = (AssemblyFileName == null);
            compilerparams.OutputAssembly = AssemblyFileName;
            compilerparams.GenerateExecutable = false;
            compilerparams.ReferencedAssemblies.Add("System.dll");
            compilerparams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerparams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerparams.ReferencedAssemblies.Add("System.Xml.dll");
            compilerparams.ReferencedAssemblies.Add("System.Collections.dll");
            foreach (var reference in References)
                compilerparams.ReferencedAssemblies.Add(reference);

            // reference this assembly to share interfaces (for debugging only)

            string tinypgfile = Assembly.GetCallingAssembly().Location;
            compilerparams.ReferencedAssemblies.Add(tinypgfile);

            if (Sources.Count > 0)
            {
                /*
                for (var i=0; i<sources.Count; ++i)
                {
                    var f = Path.GetFullPath("" + i + ".cs");
                    var src = sources[i];
                    System.IO.File.WriteAllText(f, src);
                }
                */

                Result = provider.CompileAssemblyFromSource(compilerparams, Sources.ToArray());

                if (Result.Errors.Count > 0)
                {
                    foreach (CompilerError o in Result.Errors)
                        Errors.Add(o.ErrorText + " on line " + o.Line.ToString());
                    throw new Exception("Errors: " + string.Join("\n", Errors));
                }
                else
                {
                    CompiledAssembly = Result.CompiledAssembly;
                    return CompiledAssembly;
                }
            }

            throw new Exception("No source code provided");
        }    
    }
}
