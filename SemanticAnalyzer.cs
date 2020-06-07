/*
  Hydra compiler - Semantic analyzer.
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

  class SemanticAnalyzer {
    public bool isFirstPass;
    public bool isVariableDefinition = false;
    public bool lookingExistance = false;
    public int whileCount = 0;
    //-----------------------------------------------------------
    public GlobalVariables GlobalVariables {
      get;
      private set;
    }
    public GlobalFunctions GlobalFunctions {
      get;
      private set;
    }
    public LocalSymbolTable TempLocalSymbolTable {
      get;
      private set;
    }
    //-----------------------------------------------------------
    public SemanticAnalyzer () {
      GlobalVariables = new GlobalVariables ();
      GlobalFunctions = new GlobalFunctions ();
    }

    //-----------------------------------------------------------
    public void Visit (Program node) {
      Visit ((dynamic) node[0]);
    }

    public void Visit (DefinitionList node) {
      VisitChildren (node);
    }

    public void Visit (VariableDefinitionList node) {
      isVariableDefinition = true;
      lookingExistance = false;
      VisitChildren (node);
      lookingExistance = true;
      isVariableDefinition = false;
    }

    public void Visit (FunctionDefinition node) {
      var functionName = node.AnchorToken.Lexeme;

      if (isFirstPass && !GlobalFunctions.Contains (functionName)) {
        GlobalFunctions[functionName] = new FunctionData () {
          arity = node[0].Count (),
          isPrimitive = false,
          RefST = new LocalSymbolTable (functionName)
        };
      } else if (!isFirstPass) {
        TempLocalSymbolTable = GlobalFunctions[functionName].RefST;
        VisitChildren (node);
        TempLocalSymbolTable = null;
      } else {
        throw new SemanticError (
          $"Function already declare: {functionName}",
          node.AnchorToken);
      }
    }

    public void Visit (IdParameterList node) {
      foreach (var parameter in node) {
        var variableName = parameter.AnchorToken.Lexeme;
        if (!TempLocalSymbolTable.Contains (variableName)) {
          TempLocalSymbolTable[variableName] = true;
        } else {
          throw new SemanticError (
            "Function declaration contains duplicated" +
            $"variable name: {variableName}",
            parameter.AnchorToken
          );
        }
      }
    }

    public void Visit (StatementList node) {
      var breakIndex = node.BreakIndex ();
      if (breakIndex != -1 && breakIndex != node.Count () - 1) {
        node.RemoveNodes (++breakIndex);
      }

      var returnIndex = node.ReturnIndex ();
      if (returnIndex != -1 && returnIndex != node.Count () - 1) {
        Console.WriteLine (returnIndex);
        node.RemoveNodes (++returnIndex);
        Console.WriteLine (node.ToStringTree ());
      }

      lookingExistance = true;
      VisitChildren (node);
      // lookingExistance = false;
    }

    public void Visit (Assignment node) {
      var variableName = node.AnchorToken.Lexeme;

      if (isVariableDefinition && isFirstPass) {
        lookingExistance = true;
        Visit ((dynamic) node[0]);
        lookingExistance = false;
        if (GlobalVariables.Contains (variableName)) {
          throw new SemanticError (
            $"Variable already declare: {variableName}",
            node.AnchorToken);
        }
        GlobalVariables.Add (new GlobalVariable(variableName, node[0].AnchorToken.Lexeme));
      } else if (isVariableDefinition && !isFirstPass) {
        if (TempLocalSymbolTable == null) return;
        lookingExistance = true;
        Visit ((dynamic) node[0]);
        lookingExistance = false;
        if (!TempLocalSymbolTable.Contains (variableName)) {
          TempLocalSymbolTable[variableName] = false;
        } else {
          throw new SemanticError (
            $"Variable already declare: {variableName}",
            node.AnchorToken);
        }
      } else {
        if (!GlobalVariables.Contains (variableName) && !TempLocalSymbolTable.Contains (variableName)) {
          throw new SemanticError (
            $"Variable is not declare: {variableName}",
            node.AnchorToken);
        } else {
          lookingExistance = true;
          Visit ((dynamic) node[0]);
          // lookingExistance = false;
        }
      }
    }

    public void Visit (PlusPlus node) {
      VisitBinaryOperator (node);
    }

    public void Visit (LessLess node) {
      VisitBinaryOperator (node);
    }

    public void Visit (PlusEqual node) {
      VisitBinaryOperator (node);
    }

    public void Visit (SubtracEqual node) {
      VisitBinaryOperator (node);
    }

    public void Visit (FunctionCall node) {
      var functionName = node.AnchorToken.Lexeme;
      VisitChildren (node);
      if (!GlobalFunctions.Contains (functionName)) {
        throw new SemanticError (
          $"The function {functionName} is not declare",
          node.AnchorToken
        );
      }
      var functionArity = GlobalFunctions[functionName].arity;
      if (functionArity != node.Count ()) {
        throw new SemanticError (
          $"The function {functionName} takes {functionArity} arguments " +
          $"({node.Count()} given)",
          node.AnchorToken
        );
      }
    }

    public void Visit (If node) {
      VisitChildren (node);
    }

    public void Visit (ElifList node) {
      VisitChildren (node);
    }

    public void Visit (Elif node) {
      VisitChildren (node);
    }

    public void Visit (Else node) {
      VisitChildren (node);
    }

    public void Visit (While node) {
      whileCount++;
      VisitChildren (node);
      whileCount--;
    }

    public void Visit (Break node) {
      if (whileCount == 0) {
        throw new SemanticError (
          "Break Statement found outside a While Statement",
          node.AnchorToken
        );
      }
    }

    public void Visit (Return node) {
      VisitChildren (node);
    }

    public void Visit (EmptyStatement node) {
      //Nada que analizar
    }

    public void Visit (Or node) {
      VisitBinaryOperator (node);
    }

    public void Visit (And node) {
      VisitBinaryOperator (node);
    }

    public void Visit (EqualTo node) {
      VisitBinaryOperator (node);
    }

    public void Visit (NotEqualTo node) {
      VisitBinaryOperator (node);
    }

    public void Visit (LessThan node) {
      VisitBinaryOperator (node);
    }

    public void Visit (LessEqualThan node) {
      VisitBinaryOperator (node);
    }

    public void Visit (GreaterThan node) {
      VisitBinaryOperator (node);
    }

    public void Visit (GreaterEqualThan node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Plus node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Subtraction node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Neg node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Times node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Divide node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Modulo node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Not node) {
      VisitBinaryOperator (node);
    }

    public void Visit (Identifier node) {
      var variableName = node.AnchorToken.Lexeme;

      if (isFirstPass && lookingExistance) {
        if (!GlobalVariables.Contains (variableName)) {
          throw new SemanticError (
            $"Variable {variableName} is not declare",
            node.AnchorToken
          );
        }
      } else if (isFirstPass && !GlobalVariables.Contains (variableName)) {
        GlobalVariables.Add (new GlobalVariable(variableName, "0"));
      } else if (!isFirstPass && lookingExistance) {
        if (!TempLocalSymbolTable.Contains (variableName) &&
          !GlobalVariables.Contains (variableName)
        ) {
          throw new SemanticError (
            $"Variable {variableName} is not declare in the current context",
            node.AnchorToken
          );
        } else {
          return;
        }
      } else if (!isFirstPass) {
        //Verificamos que no estemos en la segunda pasada.
        //Si seguimos en la primera significa que hubo un error
        if (TempLocalSymbolTable == null) {
          return;
        } else if (!TempLocalSymbolTable.Contains (variableName)) {
          TempLocalSymbolTable[variableName] = false;
        } else {
          throw new SemanticError (
            $"Variable already declare: {variableName}",
            node.AnchorToken);
        }
      } else {
        throw new SemanticError (
          $"Variable already declare: {variableName}",
          node.AnchorToken);
      }
    }

    public void Visit (IntLiteral node) {
      var intStr = node.AnchorToken.Lexeme;
      try {
        Convert.ToInt32 (intStr);
      } catch (OverflowException) {
        throw new SemanticError (
          $"Integer literal too large: {intStr}",
          node.AnchorToken);
      }
    }

    public void Visit (CharLiteral node) {
      //Al final todo es un int así que no hace falta
      //analizar
    }

    public void Visit (StringLiteral node) {
      //Al final todo es un int así que no hace falta
      //analizar
    }

    public void Visit (Array node) {
      VisitChildren (node);
    }

    public void Visit (True node) {
      //Al final todo es un int así que no hace falta
      //analizar
    }

    public void Visit (False node) {
      //Al final todo es un int así que no hace falta
      //analizar
    }
    //-----------------------------------------------------------
    void VisitChildren (Node node) {
      foreach (var n in node) {
        Visit ((dynamic) n);
      }
    }

    //-----------------------------------------------------------
    void VisitBinaryOperator (Node node) {
      VisitChildren (node);
    }
  }
}