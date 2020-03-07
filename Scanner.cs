using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Hydra_compiler
{
    public class Scanner
    {
        // | (?<False>           false     )
        //       | (?<True>            true      )
        static readonly Regex regex =  new Regex(
            @"
                (?<And>             [&][&]    )
              | (?<BlockComment>    (\/\*(\*(?!\/)|[^*])*\*\/) ) 
              | (?<Comment>         [/][/].*  )
              | (?<EOL>             [;]       )
              | (?<Comma>           [,]       )
              | (?<Identifier>      [a-zA-Z]\w* )   
              | (?<CharLiteral>    (['].['])|([\\][nrt'""\\])|([\\][u][a-fA-f0-9]{6}))
              | (?<StringLiteral>  ([""].*[""]))
              | (?<IntLiteral>      \d+       )
              | (?<PlusPlus>        [+][+]    )
              | (?<LessLess>        [-][-]    )
              | (?<PlusEqual>       [+][=]    )
              | (?<SubtracEqual>    [-][=]    )
              | (?<GreaterEqual>    [>][=]    )
              | (?<LessEqual>       [<][=]    )
              | (?<Plus>            [+]       )
              | (?<Times>           [*]       )
              | (?<Open_Par>        [(]       )
              | (?<Close_Par>       [)]       )
              | (?<Open_Curly>      [{]       )
              | (?<Close_Curly>     [}]       )
              | (?<Open_Brac>       [[]       )
              | (?<Close_Brac>      \]        )
              | (?<Neg>             [-]       )
              | (?<Div>             [/]       )
              | (?<Mod>             [%]       )
              | (?<EqualTo>         [=][=]    )
              | (?<NotEqualTo>      [!][=]    )
              | (?<Not>             [!]       )
              | (?<Greater>         [>]       )
              | (?<Less>            [<]       )
              | (?<Assign>          [=]       )
              | (?<Newline>         \n        )
              | (?<WhiteSpace>      \s        )     # Must go anywhere after Newline.
              | (?<Other>           .         )     # Must be last: match any other character.
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
        );

        readonly string input;

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"elif", TokenCategory.ELIF},
                {"else", TokenCategory.ELSE},
                {"false", TokenCategory.FALSE},
                {"if", TokenCategory.IF},
                {"return", TokenCategory.RETURN},
                {"true", TokenCategory.TRUE},
                {"var", TokenCategory.VAR},
                {"while", TokenCategory.WHILE},
                {"main", TokenCategory.MAIN},
                {"printi", TokenCategory.PRINTI},
                {"printc", TokenCategory.PRINTC},
                {"prints", TokenCategory.PRINTS},
                {"println", TokenCategory.PRINTLN},
                {"readi", TokenCategory.READI},
                {"reads", TokenCategory.READS},
                {"new", TokenCategory.NEW},
                {"size", TokenCategory.SIZE},
                {"add", TokenCategory.ADD},
                {"get", TokenCategory.GET},
                {"set", TokenCategory.SET}
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"IntLiteral", TokenCategory.INT_LITERAL},
                {"CharLiteral", TokenCategory.CHAR_LITERAL},
                {"StringLiteral", TokenCategory.STRING_LITERAL},
                {"Plus", TokenCategory.PLUS},
                {"Times", TokenCategory.TIMES},
                {"Open_Par", TokenCategory.OPEN_PAR},
                {"Close_Par", TokenCategory.CLOSE_PAR},
                {"Neg", TokenCategory.NEG},
                {"Div", TokenCategory.DIV},
                {"Mod", TokenCategory.MOD},
                {"Not", TokenCategory.NOT},
                {"And", TokenCategory.AND},
                {"Or", TokenCategory.OR},
                {"EqualTo", TokenCategory.EQUALTO},
                {"NotEqualTo", TokenCategory.NOTEQUALTO},
                {"Greater", TokenCategory.GREATER},
                {"Less", TokenCategory.LESS},
                {"GreaterEqual", TokenCategory.GREATEREQUAL},
                {"LessEqual", TokenCategory.LESSEQUAL},
                {"PlusPlus", TokenCategory.PLUSPLUS},
                {"LessLess", TokenCategory.LESSLESS},
                {"PlusEqual", TokenCategory.PLUSEQUAL},
                {"SubtracEqual", TokenCategory.SUBTRACEQUAL},
                {"Open_Curly", TokenCategory.OPEN_CURLY},
                {"Close_Curly", TokenCategory.CLOSE_CURLY},
                {"Open_Brac", TokenCategory.OPEN_BRAC},
                {"Close_Brac", TokenCategory.CLOSE_BRAC},
                {"Assign", TokenCategory.ASSIGN},
                {"EOL", TokenCategory.EOL},
                {"Comma", TokenCategory.COMMA},
                
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(tc, m.Value, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {
                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;
                } else if(m.Groups["BlockComment"].Success){
                    row+=m.Value.Split('\n').Length - 1;
                } else if (m.Groups["WhiteSpace"].Success
                    || m.Groups["Comment"].Success) {
                    // Skip white space and comments.
                } else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {
                        // Matched string is a Buttercup keyword.
                        yield return newTok(m, keywords[m.Value]);
                    } else {
                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }
                } else if (m.Groups["Other"].Success) {
                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.BAD_TOKEN);
                } else {
                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }
            yield return new Token(TokenCategory.EOF,null,row,input.Length - columnStart + 1);
        }





    }
}