/*
  Hydra compiler - Specific node subclasses for the AST (Abstract
  Syntax Tree).
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

namespace Hydra_compiler {

  class Program : Node { }

  class DefinitionList : Node { }

  class VariableDefinitionList : Node { }

  class FunctionDefinition : Node { }
  class IdParameterList : Node { }

  class StatementList : Node { }

  class Assignment : Node { }
  class PlusPlus : Node { }
  class LesLess : Node { }
  class PlusEqual : Node { }
  class SubtracEqual : Node { }
  class FunctionCall : Node { }
  class If : Node { }
  class ElifList : Node { }
  class Elif : Node { }
  class Else : Node { }
  class While : Node { }
  class Break : Node { }
  class Return : Node { }
  class EmptyStatement : Node { }

  class ExpressionList : Node { }
  class Or : Node { }
  class And : Node { }
  class EqualTo : Node { }
  class NotEqualTo : Node { }
  class LessThan: Node { }
  class LessEqualThan: Node { }
  class GreaterThan: Node { }
  class GreaterEqualThan: Node { }
  
  class Plus : Node { }
  class Subtraction : Node { }  
  class Neg : Node { }
  class Times : Node { }
  class Divide : Node { }
  class Modulo : Node { }
  class Not : Node { }

  class Identifier : Node { }
  class IntLiteral : Node { }
  class CharLiteral : Node { }
  class StringLiteral : Node { }

  class Array : Node { }
  class True : Node { }
  class False : Node { }
}