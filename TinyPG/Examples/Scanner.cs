// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;



namespace TinyPG
{
    #region Scanner

    public partial class Scanner
    {
        public string Input;
        public int StartPos = 0;
        public int EndPos = 0;
        public string CurrentFile;
        public int CurrentLine;
        public int CurrentColumn;
        public int CurrentPosition;
        public List<Token> Skipped; // tokens that were skipped
        public Dictionary<TokenType, Regex> Patterns;

        private Token LookAheadToken;
        private List<TokenType> Tokens;
        private List<TokenType> SkipList; // tokens to be skipped
#pragma warning disable CS0649 // Field 'Scanner.FileAndLine' is never assigned to, and will always have its default value
        private readonly TokenType FileAndLine;
#pragma warning restore CS0649 // Field 'Scanner.FileAndLine' is never assigned to, and will always have its default value

        public Scanner()
        {
            Regex regex;
            Patterns = new Dictionary<TokenType, Regex>();
            Tokens = new List<TokenType>();
            LookAheadToken = null;
            Skipped = new List<Token>();

            SkipList = new List<TokenType>();
            SkipList.Add(TokenType.WS);
            SkipList.Add(TokenType.LINECOMMENT);
            SkipList.Add(TokenType.COMMENTBLOCK);
            SkipList.Add(TokenType.REGIONCOMMENT);
            SkipList.Add(TokenType.ENDREGIONCOMMENT);

            regex = new Regex(@"public|private|protected|internal", RegexOptions.Compiled);
            Patterns.Add(TokenType.MODIFIER_KEYWORD, regex);
            Tokens.Add(TokenType.MODIFIER_KEYWORD);

            regex = new Regex(@"partial", RegexOptions.Compiled);
            Patterns.Add(TokenType.PARTIAL_KEYWORD, regex);
            Tokens.Add(TokenType.PARTIAL_KEYWORD);

            regex = new Regex(@"using", RegexOptions.Compiled);
            Patterns.Add(TokenType.USING_KEYWORD, regex);
            Tokens.Add(TokenType.USING_KEYWORD);

            regex = new Regex(@"namespace", RegexOptions.Compiled);
            Patterns.Add(TokenType.NAMESPACE_KEYWORD, regex);
            Tokens.Add(TokenType.NAMESPACE_KEYWORD);

            regex = new Regex(@"class", RegexOptions.Compiled);
            Patterns.Add(TokenType.CLASS_KEYWORD, regex);
            Tokens.Add(TokenType.CLASS_KEYWORD);

            regex = new Regex(@"get|set", RegexOptions.Compiled);
            Patterns.Add(TokenType.PROPERTY_KEYWORD, regex);
            Tokens.Add(TokenType.PROPERTY_KEYWORD);

            regex = new Regex(";", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOS, regex);
            Tokens.Add(TokenType.EOS);

            regex = new Regex(":", RegexOptions.Compiled);
            Patterns.Add(TokenType.INHERIT, regex);
            Tokens.Add(TokenType.INHERIT);

            regex = new Regex(",", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMA, regex);
            Tokens.Add(TokenType.COMMA);

            regex = new Regex(@"=", RegexOptions.Compiled);
            Patterns.Add(TokenType.ASSIGN, regex);
            Tokens.Add(TokenType.ASSIGN);

            regex = new Regex(@"{", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACEOPEN, regex);
            Tokens.Add(TokenType.BRACEOPEN);

            regex = new Regex(@"}", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACECLOSE, regex);
            Tokens.Add(TokenType.BRACECLOSE);

            regex = new Regex(@"\(", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACKETOPEN, regex);
            Tokens.Add(TokenType.BRACKETOPEN);

            regex = new Regex(@"\)", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRACKETCLOSE, regex);
            Tokens.Add(TokenType.BRACKETCLOSE);

            regex = new Regex(@"\[", RegexOptions.Compiled);
            Patterns.Add(TokenType.SQUAREOPEN, regex);
            Tokens.Add(TokenType.SQUAREOPEN);

            regex = new Regex(@"\]", RegexOptions.Compiled);
            Patterns.Add(TokenType.SQUARECLOSE, regex);
            Tokens.Add(TokenType.SQUARECLOSE);

            regex = new Regex(@"([a-zA-Z0-9_]+(\.)?)+", RegexOptions.Compiled);
            Patterns.Add(TokenType.NAMESPACE_REFERENCE, regex);
            Tokens.Add(TokenType.NAMESPACE_REFERENCE);

            regex = new Regex(@"([a-zA-Z0-9_.\[\]]+)(?=(\s+[a-zA-Z_]))", RegexOptions.Compiled);
            Patterns.Add(TokenType.TYPE, regex);
            Tokens.Add(TokenType.TYPE);

            regex = new Regex(@"[a-zA-Z0-9_]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.IDENTIFIER, regex);
            Tokens.Add(TokenType.IDENTIFIER);

            regex = new Regex(@"^$", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOF, regex);
            Tokens.Add(TokenType.EOF);

            regex = new Regex(@"[\s\n\t]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.WS, regex);
            Tokens.Add(TokenType.WS);

            regex = new Regex(@"//[^\n]*\n?", RegexOptions.Compiled);
            Patterns.Add(TokenType.LINECOMMENT, regex);
            Tokens.Add(TokenType.LINECOMMENT);

            regex = new Regex(@"/\*[^*]*\*+(?:[^/*][^*]*\*+)*/", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMENTBLOCK, regex);
            Tokens.Add(TokenType.COMMENTBLOCK);

            regex = new Regex(@"#region[^\n]*\n?", RegexOptions.Compiled);
            Patterns.Add(TokenType.REGIONCOMMENT, regex);
            Tokens.Add(TokenType.REGIONCOMMENT);

            regex = new Regex(@"#endregion[^\n]*\n?", RegexOptions.Compiled);
            Patterns.Add(TokenType.ENDREGIONCOMMENT, regex);
            Tokens.Add(TokenType.ENDREGIONCOMMENT);

            regex = new Regex(@"if[^{};]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.IF_CONDITION, regex);
            Tokens.Add(TokenType.IF_CONDITION);

            regex = new Regex(@"while[^{};]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.WHILE_LOOP, regex);
            Tokens.Add(TokenType.WHILE_LOOP);

            regex = new Regex(@"foreach[^{};]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.FOREACH_LOOP, regex);
            Tokens.Add(TokenType.FOREACH_LOOP);

            regex = new Regex(@"for[^)]+\)", RegexOptions.Compiled);
            Patterns.Add(TokenType.FOR_LOOP, regex);
            Tokens.Add(TokenType.FOR_LOOP);

            regex = new Regex(@"[^\s\n\t{};]*[^{};\n]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.ANYTHING, regex);
            Tokens.Add(TokenType.ANYTHING);


        }

        public void Init(string input)
        {
            Init(input, "");
        }

        public void Init(string input, string fileName)
        {
            this.Input = input;
            StartPos = 0;
            EndPos = 0;
            CurrentFile = fileName;
            CurrentLine = 1;
            CurrentColumn = 1;
            CurrentPosition = 0;
            LookAheadToken = null;
        }

        public Token GetToken(TokenType type)
        {
            Token t = new Token(this.StartPos, this.EndPos);
            t.Type = type;
            return t;
        }

         /// <summary>
        /// executes a lookahead of the next token
        /// and will advance the scan on the input string
        /// </summary>
        /// <returns></returns>
        public Token Scan(params TokenType[] expectedtokens)
        {
            Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
            LookAheadToken = null; // reset lookahead token, so scanning will continue
            StartPos = tok.EndPos;
            EndPos = tok.EndPos; // set the tokenizer to the new scan position
            CurrentLine = tok.Line + (tok.Text.Length - tok.Text.Replace("\n", "").Length);
            CurrentFile = tok.File;
            return tok;
        }

        /// <summary>
        /// returns token with longest best match
        /// </summary>
        /// <returns></returns>
        public Token LookAhead(params TokenType[] expectedtokens)
        {
            int i;
            int startpos = StartPos;
            int endpos = EndPos;
            int currentline = CurrentLine;
            string currentFile = CurrentFile;
            Token tok = null;
            List<TokenType> scantokens;


            // this prevents double scanning and matching
            // increased performance
            if (LookAheadToken != null 
                && LookAheadToken.Type != TokenType._UNDETERMINED_ 
                && LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

            // if no scantokens specified, then scan for all of them (= backward compatible)
            if (expectedtokens.Length == 0)
                scantokens = Tokens;
            else
            {
                scantokens = new List<TokenType>(expectedtokens);
                scantokens.AddRange(SkipList);
            }

            do
            {

                int len = -1;
                TokenType index = (TokenType)int.MaxValue;
                string input = Input.Substring(startpos);

                tok = new Token(startpos, endpos);

                for (i = 0; i < scantokens.Count; i++)
                {
                    Regex r = Patterns[scantokens[i]];
                    Match m = r.Match(input);
                    if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len )))
                    {
                        len = m.Length;
                        index = scantokens[i];  
                    }
                }

                if (index >= 0 && len >= 0)
                {
                    tok.EndPos = startpos + len;
                    tok.Text = Input.Substring(tok.StartPos, len);
                    tok.Type = index;
                }
                else if (tok.StartPos == tok.EndPos)
                {
                    if (tok.StartPos < Input.Length)
                        tok.Text = Input.Substring(tok.StartPos, 1);
                    else
                        tok.Text = "EOF";
                }

                // Update the line and column count for error reporting.
                tok.File = currentFile;
                tok.Line = currentline;
                if (tok.StartPos < Input.Length)
                    tok.Column = tok.StartPos - Input.LastIndexOf('\n', tok.StartPos);

                if (SkipList.Contains(tok.Type))
                {
                    startpos = tok.EndPos;
                    endpos = tok.EndPos;
                    currentline = tok.Line + (tok.Text.Length - tok.Text.Replace("\n", "").Length);
                    currentFile = tok.File;
                    Skipped.Add(tok);
                }
                else
                {
                    // only assign to non-skipped tokens
                    tok.Skipped = Skipped; // assign prior skips to this token
                    Skipped = new List<Token>(); //reset skips
                }

                // Check to see if the parsed token wants to 
                // alter the file and line number.
                if (tok.Type == FileAndLine)
                {
                    var match = Patterns[tok.Type].Match(tok.Text);
                    var fileMatch = match.Groups["File"];
                    if (fileMatch.Success)
                        currentFile = fileMatch.Value.Replace("\\\\", "\\");
                    var lineMatch = match.Groups["Line"];
                    if (lineMatch.Success)
                        currentline = int.Parse(lineMatch.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
            }
            while (SkipList.Contains(tok.Type));

            LookAheadToken = tok;
            return tok;
        }
    }

    #endregion

    #region Token

    public enum TokenType
    {

            //Non terminal tokens:
            _NONE_  = 0,
            _UNDETERMINED_= 1,

            //Non terminal tokens:
            Start   = 2,
            Program = 3,
            Usings  = 4,
            UsingStatement= 5,
            Namespace= 6,
            NamespaceBody= 7,
            ClassDeclaration= 8,
            ClassBody= 9,
            Declaration= 10,
            Assigment= 11,
            Property= 12,
            TypedIndexer= 13,
            Method  = 14,
            CodeBlock= 15,
            Statements= 16,
            Statement= 17,
            ParamDeclarations= 18,
            TypedParameter= 19,
            SimpleStatement= 20,
            IfStatement= 21,
            ForeachStatement= 22,
            ForStatement= 23,

            //Terminal tokens:
            MODIFIER_KEYWORD= 24,
            PARTIAL_KEYWORD= 25,
            USING_KEYWORD= 26,
            NAMESPACE_KEYWORD= 27,
            CLASS_KEYWORD= 28,
            PROPERTY_KEYWORD= 29,
            EOS     = 30,
            INHERIT = 31,
            COMMA   = 32,
            ASSIGN  = 33,
            BRACEOPEN= 34,
            BRACECLOSE= 35,
            BRACKETOPEN= 36,
            BRACKETCLOSE= 37,
            SQUAREOPEN= 38,
            SQUARECLOSE= 39,
            NAMESPACE_REFERENCE= 40,
            TYPE    = 41,
            IDENTIFIER= 42,
            EOF     = 43,
            WS      = 44,
            LINECOMMENT= 45,
            COMMENTBLOCK= 46,
            REGIONCOMMENT= 47,
            ENDREGIONCOMMENT= 48,
            IF_CONDITION= 49,
            WHILE_LOOP= 50,
            FOREACH_LOOP= 51,
            FOR_LOOP= 52,
            ANYTHING= 53
    }

    public class Token
    {
        private string file;
        private int line;
        private int column;
        private int startpos;
        private int endpos;
        private string text;
        private object value;

        // contains all prior skipped symbols
        private List<Token> skipped;

        public string File { 
            get { return file; } 
            set { file = value; }
        }

        public int Line { 
            get { return line; } 
            set { line = value; }
        }

        public int Column {
            get { return column; } 
            set { column = value; }
        }

        public int StartPos { 
            get { return startpos;} 
            set { startpos = value; }
        }

        public int Length { 
            get { return endpos - startpos;} 
        }

        public int EndPos { 
            get { return endpos;} 
            set { endpos = value; }
        }

        public string Text { 
            get { return text;} 
            set { text = value; }
        }

        public List<Token> Skipped { 
            get { return skipped;} 
            set { skipped = value; }
        }
        public object Value { 
            get { return value;} 
            set { this.value = value; }
        }

        [XmlAttribute]
        public TokenType Type;

        public Token()
            : this(0, 0)
        {
        }

        public Token(int start, int end)
        {
            Type = TokenType._UNDETERMINED_;
            startpos = start;
            endpos = end;
            Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
            Value = null;
        }

        public void UpdateRange(Token token)
        {
            if (token.StartPos < startpos) startpos = token.StartPos;
            if (token.EndPos > endpos) endpos = token.EndPos;
        }

        public override string ToString()
        {
            if (Text != null)
                return Type.ToString() + " '" + Text + "'";
            else
                return Type.ToString();
        }
    }

    #endregion
}
