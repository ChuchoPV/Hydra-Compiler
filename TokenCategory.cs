namespace Hydra_compiler
{
    public enum TokenCategory
    {
        //Keywords
        // MAIN,
        BREAK, ELIF, ELSE, FALSE, IF, RETURN, TRUE, VAR, WHILE,
        //IDENTIFIER
        ID,
        //Extras
        SEMICOLON, EOF, BAD_TOKEN,
        //LITERALS
        LIT_INT, LIT_CHAR, LIT_STR,
        //Operators
        PLUS, TIMES, OPEN_PAR, CLOSE_PAR, NEG, DIV, MOD, NOT, AND, OR, EQUALTO, NOTEQUALTO, GREATER,
        LESS, GREATEREQUAL, LESSEQUAL, PLUSPLUS, LESSLESS, PLUSEQUAL, SUBTRACEQUAL, OPEN_CURLY, CLOSE_CURLY,
        OPEN_BRAC, CLOSE_BRAC, ASSIGN, COMMA
    }
}