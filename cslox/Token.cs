using System;
using System.Collections.Generic;

namespace cslox
{
    public class Token
    {
        public TokenType type;
        public string lexeme;
        public object literal;
        public int line;
        public Token(TokenType type, string lexeme, object? literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = (literal == null) ? "null" : literal;
            this.line = line;
        }
        public override string ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}