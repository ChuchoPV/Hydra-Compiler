/*
  Hydra compiler - Token class for the scanner.
  Copyright (C) 2013-2020 Ariel Ortiz, ITESM CEM,
  Modified by: Gerardo Galván, Jesús Perea, Jorge López.

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
using System;

namespace Hydra_compiler
{
    public class Token
    {
        public TokenCategory Category { get; }
        public String Lexeme     { get; set;}
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
            return $"[{Category}, \"{Lexeme}\", @({Row}, {Column})]";
        }
    }
}