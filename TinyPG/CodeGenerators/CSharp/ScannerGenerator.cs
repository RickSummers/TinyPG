using System;
using System.Text;
using System.IO;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators.CSharp
{
    public class ScannerGenerator : BaseGenerator, ICodeGenerator
    {
        internal ScannerGenerator()
            : base("Scanner.cs")
        {
        }

        public string Generate(IGrammar iGrammar, bool Debug, bool NullableContext)
        {
            var grammar = iGrammar as Grammar;
            if (string.IsNullOrEmpty(grammar.GetTemplatePath()))
                return null;

            string scanner = File.ReadAllText(grammar.GetTemplatePath() + templateName);

            int counter = 2;
            StringBuilder tokentype = new StringBuilder();
            StringBuilder regexps = new StringBuilder();
            StringBuilder skiplist = new StringBuilder();

            foreach (TerminalSymbol s in grammar.SkipSymbols)
            {
                skiplist.AppendLine("            SkipList.Add(TokenType." + s.Name + ");");
            }

            if (grammar.FileAndLine != null)
                skiplist.AppendLine("            FileAndLine = TokenType." + grammar.FileAndLine.Name + ";");

            // build system tokens
            tokentype.AppendLine("\r\n            //Non terminal tokens:");
            tokentype.AppendLine(Helper.Outline("_NONE_", 3, "= 0,", 5));
            tokentype.AppendLine(Helper.Outline("_UNDETERMINED_", 3, "= 1,", 5));

            // build non terminal tokens
            tokentype.AppendLine("\r\n            //Non terminal tokens:");
            foreach (Symbol s in grammar.GetNonTerminals())
            {
                tokentype.AppendLine(Helper.Outline(s.Name, 3, "= " + String.Format("{0:d},", counter), 5));
                counter++;
            }

            // build terminal tokens
            tokentype.AppendLine("\r\n            //Terminal tokens:");
            bool first = true;
            foreach (TerminalSymbol s in grammar.GetTerminals())
            {
                regexps.Append("            regex = new Regex(" + s.Expression.ToString() + ", RegexOptions.Compiled");

                if (s.Attributes.ContainsKey("IgnoreCase"))
                    regexps.Append(" | RegexOptions.IgnoreCase");

                regexps.Append(");\r\n");

                regexps.Append("            Patterns.Add(TokenType." + s.Name + ", regex);\r\n");
                regexps.Append("            Tokens.Add(TokenType." + s.Name + ");\r\n\r\n");

                if (first) first = false;
                else tokentype.AppendLine(",");

                tokentype.Append(Helper.Outline(s.Name, 3, "= " + String.Format("{0:d}", counter), 5));
                counter++;
            }

            scanner = scanner.Replace(@"<%SkipList%>", skiplist.ToString());
            scanner = scanner.Replace(@"<%RegExps%>", regexps.ToString());
            scanner = scanner.Replace(@"<%TokenType%>", tokentype.ToString());
            scanner = scanner.Replace(@"<%?%>", NullableContext ? "?" : "");
            scanner = scanner.Replace(@"<%!%>", NullableContext ? "!" : "");
            scanner = scanner.Replace(@"<%NullableContext%>", NullableContext ? "#nullable enable" : "");

            if (Debug)
            {
                scanner = scanner.Replace(@"<%Namespace%>", "TinyPG.Debug");
                scanner = scanner.Replace(@"<%IToken%>", " : TinyPG.Debug.IToken");
            }
            else
            {
                scanner = scanner.Replace(@"<%Namespace%>", grammar.Directives["TinyPG"]["Namespace"]);
                scanner = scanner.Replace(@"<%IToken%>", "");
            }

            return scanner;
        }
    }
}
