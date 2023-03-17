using System;
using System.Collections.Generic;
using CodeDom = System.CodeDom.Compiler;
using System.Reflection;

using TinyPG.CodeGenerators;
using TinyPG.Debug;

using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.CodeDom.Compiler;

namespace TinyPG.Compiler
{
    public class Compiler
    {
        private Grammar Grammar;

        /// <summary>
        /// indicates if the grammar was parsed successfully
        /// </summary>
        public bool IsParsed { get; set; }

        /// <summary>
        /// indicates if the grammar was compiled successfully
        /// </summary>
        public bool IsCompiled { get; set; }

        /// <summary>
        /// a string containing the actual generated code for the scanner
        /// </summary>
        public string ScannerCode { get; set; }

        /// <summary>
        /// a string containing the actual generated code for the parser
        /// </summary>
        public string ParserCode { get; set; }

        /// <summary>
        /// a string containing the actual generated code for the Parse tree
        /// </summary>
        public string ParseTreeCode { get; set; }

        /// <summary>
        /// a list of errors that occured during parsing or compiling
        /// </summary>
        public List<string> Errors { get; set; }

        // the resulting compiled assembly
        public Assembly CompiledAssembly;


        public Compiler()
        {
            IsCompiled = false;
            Errors = new List<string>();
        }

        public void Compile(Grammar grammar)
        {
            IsParsed = false;
            IsCompiled = false;
            Errors = new List<string>();
            if (grammar == null) throw new ArgumentNullException("grammar", "Grammar may not be null");

            Grammar = grammar;
            grammar.Preprocess();
            IsParsed = true;

            BuildCode();
            if (Errors.Count == 0)
                IsCompiled = true;
        }

        /*
        /// <summary>
        /// once the grammar compiles correctly, the code can be built.
        /// </summary>
        
        private void BuildCode()
        {
            var helper = new TinyPG.CompileHelper();
            helper.Language = Grammar.Directives["TinyPG"]["Language"];
            helper.NullableContext = (Grammar.Directives["TinyPG"]["NullableContext"] == "enable");

            /*
            CodeDom.CompilerResults Result;
            CodeDom.CodeDomProvider provider = CodeGeneratorFactory.CreateCodeDomProvider(language);
            System.CodeDom.Compiler.CompilerParameters compilerparams = new System.CodeDom.Compiler.CompilerParameters();
            compilerparams.GenerateInMemory = true;
            compilerparams.GenerateExecutable = false;
            compilerparams.ReferencedAssemblies.Add("System.dll");
            compilerparams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerparams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerparams.ReferencedAssemblies.Add("System.Xml.dll");

            // reference this assembly to share interfaces (for debugging only)

            string tinypgfile = Assembly.GetExecutingAssembly().Location;
            compilerparams.ReferencedAssemblies.Add(tinypgfile);

            // generate the code with debug interface enabled
            List<string> sources = new List<string>();
            *

            var ds = new string[] {"Parser", "Scanner", "ParseTree", "TextHighlighter" };
            var cg = new ICodeGenerator[ds.Length];
            for (var i=0; i<ds.Length; i++)
            {
                var directive = Grammar.Directives[ds[i]];
                cg[i] = CreateGenerator(ds[i], helper.Language);
                if (cg[i] != null) 
                {
                    if (directive.ContainsKey("FileName"))
                        cg[i].FileName = directive["FileName"];

                    if (directive["Generate"].ToLower() == "true")
                    {
                        var source = cg[i].Generate(Grammar, true, helper.NullableContext);
                        helper.Sources.Add(source);
                    }
                }
            }

            try
            {
                assembly = helper.BuildCode();
            }
            catch (Exception)
            {
                Errors = helper.Errors;
            }
        }
        */

        /// <summary>
        /// once the grammar compiles correctly, the code can be built.
        /// </summary>
        private Assembly BuildCode()
        {
            var language = Grammar.Directives["TinyPG"]["Language"];
            var nullableContext = (Grammar.Directives["TinyPG"]["NullableContext"] == "enable");
            var sources = new List<string>();

            var ds = new string[] {"Parser", "Scanner", "ParseTree", "TextHighlighter" };
            var cg = new TinyPG.CodeGenerators.ICodeGenerator[ds.Length];
            for (var i=0; i<ds.Length; i++)
            {
                var directive = Grammar.Directives[ds[i]];
                cg[i] = CreateGenerator(ds[i], language);
                if (cg[i] != null) 
                {
                    if (directive.ContainsKey("FileName"))
                        cg[i].FileName = directive["FileName"];

                    if (directive["Generate"].ToLower() == "true")
                    {
                        var source = cg[i].Generate(Grammar, true, nullableContext);
                        sources.Add(source);
                    }
                }
            }

            var assemblyFileName = Path.GetFullPath("assembly");
//          if (File.Exists(assemblyFileName))
//              File.Delete(assemblyFileName);
            var errorFileName = Path.GetFullPath("errors.txt");
            if (File.Exists(errorFileName))
                File.Delete(errorFileName);

            using (var process = new Process())
            {
                process.StartInfo.FileName = @"C:\Github\TinyPG\TinyPG48\bin\Debug\TinyPG48.exe";
                process.StartInfo.Arguments = "-a \"" + assemblyFileName + "\" -e \"" + errorFileName + "\"";
                if (!nullableContext) process.StartInfo.Arguments += " -n-";
                process.StartInfo.Arguments += " -r \"" + Assembly.GetExecutingAssembly().Location + "\"";
                foreach (var source in sources)
                {
                    process.StartInfo.Arguments += " ";
                    var filename = Path.GetTempFileName();
                    File.WriteAllText(filename, source);
                    process.StartInfo.Arguments += "\"" + filename + "\"";
                }
                System.Diagnostics.Debug.WriteLine(process.StartInfo.Arguments);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true; //not diplay a windows
                process.Start();
                string output = process.StandardOutput.ReadToEnd(); //The output result
                process.WaitForExit();
            }

            if (File.Exists(errorFileName) && new FileInfo(errorFileName).Length > 0)
            {
                Errors.AddRange(File.ReadAllLines(errorFileName));
                throw new Exception("Errors: " + string.Join("\r\n", Errors));
            }
            else
            {
                CompiledAssembly = Assembly.LoadFile(assemblyFileName);
                return CompiledAssembly;
            }
        }

        internal static TinyPG.CodeGenerators.ICodeGenerator CreateGenerator(string generator, string language)
        {
            switch (CodeGeneratorFactory.GetSupportedLanguage(language))
            {
                // set the default templates directory
                case SupportedLanguage.VBNet:
                    switch (generator)
                    {
                        case "Parser":
                            return new TinyPG.CodeGenerators.VBNet.ParserGenerator();
                        case "Scanner":
                            return new TinyPG.CodeGenerators.VBNet.ScannerGenerator();
                        case "ParseTree":
                            return new TinyPG.CodeGenerators.VBNet.ParseTreeGenerator();
                        case "TextHighlighter":
                            return new TinyPG.CodeGenerators.VBNet.TextHighlighterGenerator();
                    }
                    break;
                default: // c# is default language
                    switch (generator)
                    {
                        case "Parser":
                            return new TinyPG.CodeGenerators.CSharp.ParserGenerator();
                        case "Scanner":
                            return new TinyPG.CodeGenerators.CSharp.ScannerGenerator();
                        case "ParseTree":
                            return new TinyPG.CodeGenerators.CSharp.ParseTreeGenerator();
                        case "TextHighlighter":
                            return new TinyPG.CodeGenerators.CSharp.TextHighlighterGenerator();
                    }
                    break;
            }
            return null; // codegenerator was not found
        }


        /// <summary>
        /// evaluate the input expression
        /// </summary>
        /// <param name="input">the expression to evaluate with the parser</param>
        /// <returns>the output of the parser/compiler</returns>
        public CompilerResult Run(string input)
        {
            return Run(input, null);
        }

        public CompilerResult Run(string input, RichTextBox textHighlight)
        {
            CompilerResult compilerresult = new CompilerResult();
            string output = null;
            if (CompiledAssembly == null) return null;

            object scannerinstance = CompiledAssembly.CreateInstance("TinyPG.Debug.Scanner");
            Type scanner = scannerinstance.GetType();

            object parserinstance = (IParser)CompiledAssembly.CreateInstance("TinyPG.Debug.Parser", true, BindingFlags.CreateInstance, null, new object[] { scannerinstance }, null, null);
            Type parsertype = parserinstance.GetType();

            object treeinstance = parsertype.InvokeMember("Parse", BindingFlags.InvokeMethod, null, parserinstance, new object[] { input, string.Empty });
            IParseTree itree = treeinstance as IParseTree;

            compilerresult.ParseTree = itree;
            Type treetype = treeinstance.GetType();

            List<IParseError> errors = (List<IParseError>)treetype.InvokeMember("Errors", BindingFlags.GetField, null, treeinstance, null);

            if (textHighlight != null && errors.Count == 0)
            {
                // try highlight the input text
                object highlighterinstance = CompiledAssembly.CreateInstance("TinyPG.Debug.TextHighlighter", true, BindingFlags.CreateInstance, null, new object[] { textHighlight, scannerinstance, parserinstance }, null, null);
                if (highlighterinstance != null)
                {
                    output += "Highlighting input..." + "\r\n";
                    Type highlightertype = highlighterinstance.GetType();
                    // highlight the input text only once
                    highlightertype.InvokeMember("HighlightText", BindingFlags.InvokeMethod, null, highlighterinstance, null);

                    // let this thread sleep so background thread can highlight the text
                    System.Threading.Thread.Sleep(20);

                    // dispose of the highlighter object
                    highlightertype.InvokeMember("Dispose", BindingFlags.InvokeMethod, null, highlighterinstance, null);
                }
            }
            if (errors.Count > 0)
            {
                foreach (IParseError err in errors)
                    output += string.Format("({0},{1}): {2}\r\n", err.Line, err.Column, err.Message);
            }
            else
            {
                output += "Parse was successful." + "\r\n";
                output += "Evaluating...";

                // parsing was successful, now try to evaluate... this should really be done on a seperate thread.
                // e.g. if the thread hangs, it will hang the entire application (!)
                try
                {
                    compilerresult.Value = itree.Eval(null);
                    output += "\r\nResult: " + (compilerresult.Value == null ? "null" : compilerresult.Value.ToString());
                }
                catch (Exception exc)
                {
                    output += "\r\nException occurred: " + exc.Message;
                    output += "\r\nStacktrace: " + exc.StackTrace;
                }

            }
            compilerresult.Output = output.ToString();
            return compilerresult;
        }
    }
}
