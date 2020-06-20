using Compiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Parser
{
    class Parse
    {
        public ParseType ParseType { get; set; }
        public string Value { get; set; }
        public int Index { get; set; }
        public string VarName { get; set; }

        public Parse(Token token)
        {
            ParseType = GetRPNTypeByTokenType(token.TokenType);
            Value = token.Value;
        }

        public Parse(string value, ParseType parseType)
        {
            this.ParseType = parseType;
            Value = value;
        }

        public Parse(string value, ParseType parseType, int index, string varName)
        {
            ParseType = parseType;
            Value = value;
            Index = index;
            VarName = varName;
        }

        public Parse(ParseOperation parseOperation)
        {
            ParseType = parseOperation.parseType;
            Value = parseOperation.Value;
        }

        private ParseType GetRPNTypeByTokenType(TokenType tokenType)
        {
            switch(tokenType)
            {
                case TokenType.A_OP:
                    return ParseType.A_OP;
                case TokenType.C_OP:
                    return ParseType.C_OP;
                case TokenType.DEC:
                    return ParseType.DEC;
                case TokenType.DIGIT:
                    return ParseType.DIGIT;
                case TokenType.INC:
                    return ParseType.INC;
                case TokenType.L_OP:
                    return ParseType.L_OP;
                case TokenType.OP:
                    return ParseType.OP;
                case TokenType.VAR:
                    return ParseType.VAR;
                case TokenType.PRINT:
                    return ParseType.PRINT;
                case TokenType.CREATE_LIST:
                    return ParseType.CREATE_LIST;
                case TokenType.CREATE_HASH_SET:
                    return ParseType.CREATE_HASH_SET;
                case TokenType.ASYNC:
                    return ParseType.ASYNC;
                default:
                    throw new Exception("match for " + tokenType + " not found");
            }
        }

        public override string ToString()
        {
            string template = "{0}";//"Type: " + ParseType + ", Value: {0}";
            switch (ParseType)
            {
                case ParseType.COND_TRAN:
                    return String.Format(template, "p"+ Value + "!F");
                case ParseType.UNCOND_TRAN:
                    return String.Format(template, "p" + Value + "!");
                case ParseType.UNCOND_TRAN_L:
                    return String.Format(template, "p" + Value + "!");
                case ParseType.LABEL:
                    return String.Format(template, "L" + Value + ":");
                default:
                    return String.Format(template, Value);
            }
        }
    }
}
