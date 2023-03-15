using System;
using System.Collections.Generic;
using System.Text;
using TinyPG.CodeGenerators;
using TinyPG.Compiler;
using System.IO;

namespace TinyPG
{
    public class GeneratedFilesWriter
    {

        private Grammar grammar = null;

        public GeneratedFilesWriter(Grammar grammar)
        {
            this.grammar = grammar;
        }

        public void Generate(bool debug)
        {

            ICodeGenerator generator;

            string language = grammar.Directives["TinyPG"]["Language"];
            bool nullableContext = (grammar.Directives["TinyPG"]["NullableContext"] == "enable");
            foreach (Directive d in grammar.Directives)
            {
                generator = TinyPG.Compiler.Compiler.CreateGenerator(d.Name, language);
                
                if (generator != null && d.ContainsKey("FileName"))
                {
                    generator.FileName = d["FileName"];
                }

                if (generator != null && d["Generate"].ToLower() == "true")
                {
                    File.WriteAllText(
                        Path.Combine(grammar.GetOutputPath(), generator.FileName),
                        generator.Generate(grammar, debug, nullableContext));
                }
            }

        }


    }
}
