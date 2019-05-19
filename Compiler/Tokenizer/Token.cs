using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler.Tokenizer
{
    class Token
    {
        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public TokenType TokenType { get; set; }
        public string Value { get; set; }

        public Token Clone()
        {
            return new Token(TokenType, Value);
        }
        public override string ToString()
        {
            return Value;//"TokenType:" + TokenType + ", Value: " + Value;
        }
    }
}
