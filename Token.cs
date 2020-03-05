using System;

namespace Hydra_compiler
{
    public class Token
    {
        public TokenCategory Category { get; }
        public String Lexeme     { get; }
        public int Row { get; }
        public int Column { get; }

        public Token(TokenCategory category, String lexeme, int row, int column)
        {
            Category = category;
            Lexeme = lexeme;
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"[{Category}, \"{Lexeme}\", {Row}, {Column}]";
        }
    }
}