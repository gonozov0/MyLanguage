using Compiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Parser
{
    class ParseOperation
    {
        public ushort Priority { get; set; }
        public ParseType parseType { get; set; }
        public string Value { get; set; }

        public ParseOperation(Token token)
        {
            Value = token.Value;
            switch (token.TokenType)
            {
                case TokenType.A_OP:
                    Priority = 1;
                    parseType = ParseType.A_OP;
                    break;
                case TokenType.C_OP:
                    Priority = 4;
                    parseType = ParseType.C_OP;
                    break;
                case TokenType.DEC:
                    Priority = 7;
                    parseType = ParseType.DEC;
                    break;
                case TokenType.INC:
                    Priority = 7;
                    parseType = ParseType.INC;
                    break;
                case TokenType.L_OP:
                    parseType = ParseType.L_OP;
                    switch (Value)
                    {
                        case "||":
                            Priority = 2;
                            break;
                        case "&&":
                            Priority = 3;
                            break;
                        default:
                            throw new Exception("operation for " + token.TokenType + " with value: " + Value + " not found");
                    }
                    break;
                case TokenType.OP:
                    parseType = ParseType.OP;
                    switch (Value)
                    {
                        case "-":
                        case "+":
                            Priority = 5;
                            break;
                        case "*":
                        case "/":
                            Priority = 6;
                            break;
                        default:
                            throw new Exception("operation for " + token.TokenType + " with value: " + Value + " not found");
                    }
                    break;
                case TokenType.LB:
                    Priority = 0;
                    parseType = ParseType.LB;
                    break;
                case TokenType.LB_S:
                    Priority = 0;
                    parseType = ParseType.LB_S;
                    break;
                case TokenType.ADD:
                    Priority = 8;
                    parseType = ParseType.ADD;
                    break;
                case TokenType.DELETE:
                    Priority = 8;
                    parseType = ParseType.DELETE;
                    break;
                case TokenType.CONTAINS:
                    Priority = 8;
                    parseType = ParseType.CONTAINS;
                    break;
                default:
                    throw new Exception("match for " + token.TokenType + " not found");
            }
        }


    }
}
