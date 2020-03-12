/*
  Hydra compiler - Token categories for the scanner.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM
  Modified by: Jesús Perea, Jorge lópez, Gerardo Galván.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace Hydra_compiler
{   
    public enum TokenCategory
    {
        //Keywords
        BREAK, ELIF, ELSE, FALSE, IF, RETURN, TRUE, VAR, WHILE, MAIN, PRINTI, PRINTC, PRINTS, PRINTLN, READI, READS,
        NEW, SIZE, ADD, GET, SET,
        //IDENTIFIER
        ID,
        //Extras
        EOL,EOF, BAD_TOKEN,
        //LITERALS
        LIT_INT, LIT_CHAR, LIT_STR,
        //Operators
        PLUS, TIMES, OPEN_PAR, CLOSE_PAR, NEG, DIV, MOD, NOT, AND, OR, EQUALTO, NOTEQUALTO, GREATER,
        LESS, GREATEREQUAL, LESSEQUAL, PLUSPLUS, LESSLESS, PLUSEQUAL, SUBTRACEQUAL, OPEN_CURLY, CLOSE_CURLY,
        OPEN_BRAC, CLOSE_BRAC, ASSIGN, COMMA
    }
}   