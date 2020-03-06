using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Hydra_compiler
{
    public class Scanner
    {
        static readonly Regex regex =  new Regex(
            @"
                (?<And>             [&][&]    )
              | (?<Assign>          [=]       )
              | (?<Comment>         ([/][/].*) | ([/][*].*?[*][/]) )
              | (?<False>           false     )
              | (?<Identifier>      [a-zA-Z]+ )
              | (?<INT_LITERAL>     \d+       )
              | (?<CHAR_LITERAL>    \d+       )
              | (?<Less>            [<]       )
              | (?<Mul>             [*]       )
              | (?<Neg>             [-]       )
              | (?<Newline>         \n        )
              | (?<ParLeft>         [(]       )
              | (?<ParRight>        [)]       )
              | (?<Plus>            [+]       )
              | (?<True>            true      )
              | (?<WhiteSpace>      \s        )     # Must go anywhere after Newline.
              | (?<Other>           .         )     # Must be last: match any other character.
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
        );
        readonly string input;

        public Scanner(string input) {
            this.input = input;
        }



    }
}