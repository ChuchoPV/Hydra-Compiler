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
using System.Collections.Generic;

namespace Hydra_compiler {

  class SemanticAnalyzer {
    public bool isFirstPass;
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
      VisitChildren (node);
    }

    public void Visit (FunctionDefinition node) {
      var functionName = node.AnchorToken.Lexeme;

      if (isFirstPass && !GlobalFunctions.Contains (functionName)) {
        GlobalFunctions[functionName] = new FunctionData () {
          arity = node[0].Count (),
          isPrimitive = false,
        };
      } else if (!isFirstPass) {
        // VisitChildren(node);
        TempLocalSymbolTable = GlobalFunctions[functionName].RefST;
        Visit ((dynamic) node[0]);
        // Visit ((dynamic) node[1]);
        // Visit ((dynamic) node[2]);
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
            "Function declaration contains duplicated"+
            $"variable name: {variableName}", 
            parameter.AnchorToken
          );
        }
      }
    }

    public void Visit (Assignment node) {
      var variableName = node.AnchorToken.Lexeme;

      if (isFirstPass && !GlobalVariables.Contains (variableName)) {
        GlobalVariables.Add (variableName);
      } else if (!isFirstPass && !TempLocalSymbolTable.Contains (variableName)) {
        TempLocalSymbolTable[variableName] = false;
      } else {
        throw new SemanticError (
          $"Variable already declare: {variableName}",
          node.AnchorToken);
      }
    }

    public void Visit (Identifier node) {
      var variableName = node.AnchorToken.Lexeme;

      if (isFirstPass && !GlobalVariables.Contains (variableName)) {
        GlobalVariables.Add (variableName);
      } else {
        throw new SemanticError (
          $"Variable already declare: {variableName}",
          node.AnchorToken);
      }
    }

    //-----------------------------------------------------------
    void VisitChildren (Node node) {
      foreach (var n in node) {
        Visit ((dynamic) n);
      }
    }

    //-----------------------------------------------------------
    void VisitBinaryOperator (char op, Node node, Type type) {
      if (Visit ((dynamic) node[0]) != type ||
        Visit ((dynamic) node[1]) != type) {
        throw new SemanticError (
          $"Operator {op} requires two operands of type {type}",
          node.AnchorToken);
      }
    }
  }
}