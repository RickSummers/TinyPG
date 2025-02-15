// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;

namespace TinyPG
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public ParseTree Parse(string input, string fileName)
        {
            tree = new ParseTree();
            return Parse(input, fileName, tree);
        }

        public ParseTree Parse(string input, string fileName, ParseTree tree)
        {
            scanner.Init(input, fileName);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.DIRECTIVEOPEN);
            while (tok.Type == TokenType.DIRECTIVEOPEN)
            {
                ParseDirective(node);
            tok = scanner.LookAhead(TokenType.DIRECTIVEOPEN);
            }

            
            tok = scanner.LookAhead(TokenType.SQUAREOPEN, TokenType.IDENTIFIER);
            while (tok.Type == TokenType.SQUAREOPEN
                || tok.Type == TokenType.IDENTIFIER)
            {
                ParseExtProduction(node);
            tok = scanner.LookAhead(TokenType.SQUAREOPEN, TokenType.IDENTIFIER);
            }

            
            tok = scanner.Scan(TokenType.EOF);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.EOF) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EOF.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseDirective(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Directive), "Directive");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.DIRECTIVEOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.DIRECTIVEOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DIRECTIVEOPEN.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.IDENTIFIER) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.IDENTIFIER);
            while (tok.Type == TokenType.IDENTIFIER)
            {
                ParseNameValue(node);
            tok = scanner.LookAhead(TokenType.IDENTIFIER);
            }

            
            tok = scanner.Scan(TokenType.DIRECTIVECLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.DIRECTIVECLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DIRECTIVECLOSE.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseNameValue(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.NameValue), "NameValue");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.IDENTIFIER) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.Scan(TokenType.ASSIGN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.ASSIGN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ASSIGN.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.Scan(TokenType.STRING);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.STRING) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseExtProduction(ParseNode parent)
        {
            Token tok;
            //ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ExtProduction), "ExtProduction");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.SQUAREOPEN);
            while (tok.Type == TokenType.SQUAREOPEN)
            {
                ParseAttribute(node);
            tok = scanner.LookAhead(TokenType.SQUAREOPEN);
            }

            
            ParseProduction(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAttribute(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Attribute), "Attribute");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.SQUAREOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.SQUAREOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SQUAREOPEN.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.IDENTIFIER) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.LookAhead(TokenType.BRACKETOPEN);
            if (tok.Type == TokenType.BRACKETOPEN)
            {

                
                tok = scanner.Scan(TokenType.BRACKETOPEN);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.BRACKETOPEN) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETOPEN.ToString(), 0x1001, tok));
                    return;
                }

                
                tok = scanner.LookAhead(TokenType.INTEGER, TokenType.DOUBLE, TokenType.STRING, TokenType.HEX);
                if (tok.Type == TokenType.INTEGER
                    || tok.Type == TokenType.DOUBLE
                    || tok.Type == TokenType.STRING
                    || tok.Type == TokenType.HEX)
                {
                    ParseParams(node);
                }

                
                tok = scanner.Scan(TokenType.BRACKETCLOSE);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.BRACKETCLOSE) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETCLOSE.ToString(), 0x1001, tok));
                    return;
                }
            }

            
            tok = scanner.Scan(TokenType.SQUARECLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.SQUARECLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SQUARECLOSE.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParams(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Params), "Params");
            parent.Nodes.Add(node);


            
            ParseParam(node);

            
            tok = scanner.LookAhead(TokenType.COMMA);
            while (tok.Type == TokenType.COMMA)
            {

                
                tok = scanner.Scan(TokenType.COMMA);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.COMMA) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.COMMA.ToString(), 0x1001, tok));
                    return;
                }

                
                ParseParam(node);
            tok = scanner.LookAhead(TokenType.COMMA);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseParam(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Param), "Param");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.INTEGER, TokenType.DOUBLE, TokenType.STRING, TokenType.HEX);
            switch (tok.Type)
            {
                case TokenType.INTEGER:
                    tok = scanner.Scan(TokenType.INTEGER);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.INTEGER) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.INTEGER.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.DOUBLE:
                    tok = scanner.Scan(TokenType.DOUBLE);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.DOUBLE) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DOUBLE.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.STRING:
                    tok = scanner.Scan(TokenType.STRING);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.STRING) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.HEX:
                    tok = scanner.Scan(TokenType.HEX);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.HEX) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.HEX.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseProduction(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Production), "Production");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.IDENTIFIER);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.IDENTIFIER) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.Scan(TokenType.ARROW);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.ARROW) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.ARROW.ToString(), 0x1001, tok));
                return;
            }

            
            ParseRule(node);

            
            tok = scanner.LookAhead(TokenType.CODEBLOCK, TokenType.SEMICOLON);
            switch (tok.Type)
            {
                case TokenType.CODEBLOCK:
                    tok = scanner.Scan(TokenType.CODEBLOCK);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.CODEBLOCK) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.CODEBLOCK.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.SEMICOLON:
                    tok = scanner.Scan(TokenType.SEMICOLON);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.SEMICOLON) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEMICOLON.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseRule(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Rule), "Rule");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.STRING, TokenType.IDENTIFIER, TokenType.BRACKETOPEN);
            switch (tok.Type)
            {
                case TokenType.STRING:
                    tok = scanner.Scan(TokenType.STRING);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.STRING) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.IDENTIFIER:
                case TokenType.BRACKETOPEN:
                    ParseSubrule(node);
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseSubrule(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Subrule), "Subrule");
            parent.Nodes.Add(node);


            
            ParseConcatRule(node);

            
            tok = scanner.LookAhead(TokenType.PIPE);
            while (tok.Type == TokenType.PIPE)
            {

                
                tok = scanner.Scan(TokenType.PIPE);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.PIPE) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PIPE.ToString(), 0x1001, tok));
                    return;
                }

                
                ParseConcatRule(node);
            tok = scanner.LookAhead(TokenType.PIPE);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseConcatRule(ParseNode parent)
        {
            Token tok;
            //ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.ConcatRule), "ConcatRule");
            parent.Nodes.Add(node);

            do {
                ParseSymbol(node);
                tok = scanner.LookAhead(TokenType.IDENTIFIER, TokenType.BRACKETOPEN);
            } while (tok.Type == TokenType.IDENTIFIER
                || tok.Type == TokenType.BRACKETOPEN);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseSymbol(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Symbol), "Symbol");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.IDENTIFIER, TokenType.BRACKETOPEN);
            switch (tok.Type)
            {
                case TokenType.IDENTIFIER:
                    tok = scanner.Scan(TokenType.IDENTIFIER);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.IDENTIFIER) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.IDENTIFIER.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.BRACKETOPEN:

                    
                    tok = scanner.Scan(TokenType.BRACKETOPEN);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.BRACKETOPEN) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETOPEN.ToString(), 0x1001, tok));
                        return;
                    }

                    
                    ParseSubrule(node);

                    
                    tok = scanner.Scan(TokenType.BRACKETCLOSE);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.BRACKETCLOSE) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRACKETCLOSE.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, tok));
                    break;
            }

            
            tok = scanner.LookAhead(TokenType.UNARYOPER);
            if (tok.Type == TokenType.UNARYOPER)
            {
                tok = scanner.Scan(TokenType.UNARYOPER);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.UNARYOPER) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.UNARYOPER.ToString(), 0x1001, tok));
                    return;
                }
            }

            parent.Token.UpdateRange(node.Token);
        }


    }

    #endregion Parser
}
