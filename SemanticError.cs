/*
  Hydra compiler - Semantic error exception class.
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

namespace Hydra_compiler {

    class SemanticError : Exception {

        public SemanticError (string message, Token token):
            base ($"Semantic Error: {message} \n" +
                $"at row {token.Row}, column {token.Column}.") { }

        public SemanticError (string message, string filename):
            base ($"Semantic Error: {message} " +
                $"at {filename}") { }
    }
}