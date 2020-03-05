namespace Hydra_compiler
{   
    public enum TokenCategory
    {
        PLUS, TIMES, OPEN_PAR, CLOSE_PAR, EOF, BAD_TOKEN,
        //Keyworkds
        BREAK, ELIF, ELSE, FALSE, IF, RETURN, TRUE, VAR, WHILE,
        //IDENTIFIER
        IDENTIFIER,
        //LITERALS
        INT, CHAR, STRING

    }
}   