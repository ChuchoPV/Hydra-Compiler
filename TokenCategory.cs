namespace Hydra_compiler
{   
    public enum TokenCategory
    {
        //Keywords
        BREAK, ELIF, ELSE, FALSE, IF, RETURN, TRUE, VAR, WHILE,
        //IDENTIFIER
        IDENTIFIER,
        //LITERALS
        INT_LITERAL, CHAR_LITERAL, STRING_LITERAL,
        //Operators
        PLUS, TIMES, OPEN_PAR, CLOSE_PAR, NEG, DIV, MOD, NOT, AND, OR, EQUALTO, NOTEQUALTO, GREATER,
        LESS, GREATEREQUAL, LESSEQUAL,
        //Extras
        EOF, BAD_TOKEN,
    }
}   