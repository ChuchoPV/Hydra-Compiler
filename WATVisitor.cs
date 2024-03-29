/*
  Hydra compiler - WebAssembly text file (WAT) code generator.
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
using System.Text;
using System.Text.RegularExpressions;

namespace Hydra_compiler {

  class WATVisitor {

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
    string tempValueAssignment = "";
    bool inVariableDefinition = false;
    int labelCounter = 0;
    List<String> tempVariables = new List<String> ();
    HashSet<FunctionData> apiUsed = new HashSet<FunctionData> ();
    Stack<String> whileStack = new Stack<String> ();

    public WATVisitor (GlobalVariables globalVariables, GlobalFunctions globalFunctions) {
      this.GlobalFunctions = globalFunctions;
      this.GlobalVariables = globalVariables;
      labelCounter = 0;
    }
    public string Generatelabel () {
      return $"${labelCounter++:00000}";
    }
    public (int code, int jumps) Unescape (string str, int i) {
      //TODO: Pobar unicodes inexistentes
      var max_code_point = Int32.Parse ("10FFFF", System.Globalization.NumberStyles.HexNumber);
      if (str[i + 1] == 'u') {
        return (
          (
            Int32.Parse (str.Substring (i + 2, 6), System.Globalization.NumberStyles.HexNumber) %
            max_code_point
          ),
          7
        );
      }
      str = str.Substring (i, 2);
      return (
        char.ConvertToUtf32 (Regex.Unescape (str), 0),
        1
      );
    }
    public IList<int> AsCodePoint (String str) {
      var result = new List<int> (str.Length);
      for (int i = 0; i < str.Length; i++) {
        if (str[i] == '\\') {
          var code_jumps = Unescape (str, i);
          result.Add (code_jumps.code);
          i += code_jumps.jumps;
          continue;
        }
        result.Add (char.ConvertToUtf32 (str, i));
        if (char.IsHighSurrogate (str, i)) {
          i++;
        }
      }
      return result;
    }
    public string ImportAPI () {
      string funcionImport = "";
      foreach (var function in apiUsed) {
        funcionImport += $"  (import \"hydra\" \"{function.name}\" (func ${function.name} ";
        for (int i = 0; i < function.arity; i++) {
          funcionImport += "(param i32) ";
        }
        funcionImport += "(result i32)))\n";
      }
      return funcionImport;
    }
    public string ImportGlobals () {
      StringBuilder sb = new StringBuilder ();
      foreach (var varName in GlobalVariables) {
        int number;
        //'c'
        //"Hola"
        var isInt = Int32.TryParse (varName.value, out number);
        sb.Append ($"\t(global ${varName.name} (mut i32) (i32.const {(isInt ? number : 0)}))\n");
      }
      return sb.ToString ();
    }

    //-----------------------------------------------------------
    public string Visit (Program node) {
      var body = VisitChildren (node);
      return ";; WebAssembly text format code generated by " +
        "the hydra compiler.\n\n" +
        "(module\n" +
        ImportAPI () +
        ImportGlobals () +
        body +
        ")\n";
    }
    //-----------------------------------------------------------
    public string Visit (DefinitionList node) {
      return VisitChildren (node);
    }
    //-----------------------------------------------------------
    public string Visit (VariableDefinitionList node) {
      inVariableDefinition = true;
      var result = VisitChildren (node);
      result += tempValueAssignment;
      tempValueAssignment = "";
      inVariableDefinition = false;
      return result;
    }
    //-----------------------------------------------------------
    public string Visit (FunctionDefinition node) {
      var functionName = node.AnchorToken.Lexeme;
      StringBuilder sb = new StringBuilder ();
      this.TempLocalSymbolTable = GlobalFunctions[functionName].RefST;
      if (functionName == "main") {
        sb.Append ($"  (func (export \"{functionName}\") ");
      } else {
        sb.Append ($"  (func ${functionName} ");
      };
      var body = Visit ((dynamic) node[1]) + Visit ((dynamic) node[2]);
      sb.Append (Visit ((dynamic) node[0]));
      sb.Append ("\n\t\t(result i32)\n");
      foreach (var tempVariable in tempVariables) {
        sb.Append ($"\t\t(local {tempVariable} i32)\n");
      }
      tempVariables = new List<string> ();
      sb.Append (body);
      sb.Append ("\t\ti32.const 0\n\t)\n");
      this.TempLocalSymbolTable = null;
      return sb.ToString ();
    }
    //-----------------------------------------------------------
    public string Visit (IdParameterList node) {
      StringBuilder sb = new StringBuilder ();
      foreach (var param in TempLocalSymbolTable) {
        if (param.Value) {
          sb.Append ($"(param ${param.Key} i32) ");
        }
      }
      return sb.ToString ();
    }
    //-----------------------------------------------------------
    public string Visit (StatementList node) {
      return VisitChildren (node);
    }
    //-----------------------------------------------------------
    public string VisitExpression (dynamic node) {
      var str = "";
      if (node is FunctionCall) {
        str += Visit ((dynamic) node, true);
      } else {
        str += Visit ((dynamic) node);
      }
      return str;
    }

    public string Visit (Assignment node) {
      var varName = node.AnchorToken.Lexeme;
      if (inVariableDefinition) {
        if (TempLocalSymbolTable != null && TempLocalSymbolTable.Contains (varName)) {
          //I'm creating a new local variable
          //and assigning a value to that local variable
          tempValueAssignment += VisitExpression (node[0]);
          tempValueAssignment += $"    local.set ${varName}\n";
          return $"    (local ${varName} i32)\n";
        } else {
          //I'm creating a new global variable
          //and assigning a value to that global variable
          //But that logic it was already proccesed
          return "";
        }
      } else {
        if (TempLocalSymbolTable.Contains (varName)) {
          //I'm ssigning a value to local variable
          return VisitExpression (node[0]) +
            $"\t\tlocal.set ${varName}\n";
        } else {
          //I'm assigning a value to a global variable
          return VisitExpression (node[0]) +
            $"\t\tglobal.set ${varName}\n";
        }
      }
    }
    //-----------------------------------------------------------
    public string Visit (PlusPlus node) {
      var str = "\t\ti32.const 1\n" +
        Visit ((dynamic) node[0]) +
        "\t\ti32.add\n";
      var varName = node[0].AnchorToken.Lexeme;
      if (TempLocalSymbolTable.Contains (varName)) {
        str += $"\t\tlocal.set ${varName}\n";
      } else {
        str += $"\t\tglobal.set ${varName}\n";
      }

      return str;
    }
    //-----------------------------------------------------------
    public string Visit (LessLess node) {
      var str = Visit ((dynamic) node[0]) +
        "\t\ti32.const 1\n" +
        "\t\ti32.sub\n";
      var varName = node[0].AnchorToken.Lexeme;
      if (TempLocalSymbolTable.Contains (varName)) {
        str += $"\t\tlocal.set ${varName}\n";
      } else {
        str += $"\t\tglobal.set ${varName}\n";
      }

      return str;
    }
    //-----------------------------------------------------------
    public string Visit (PlusEqual node) {
      var str = Visit ((dynamic) node[0]);
      str += VisitExpression ((dynamic) node[1]);
      var varName = node[0].AnchorToken.Lexeme;
      str += "\t\ti32.add\n";
      if (TempLocalSymbolTable.Contains (varName)) {
        str += $"\t\tlocal.set ${varName}\n";
      } else {
        str += $"\t\tglobal.set ${varName}\n";
      }
      return str;
    }
    //-----------------------------------------------------------
    public string Visit (SubtracEqual node) {
      var str = Visit ((dynamic) node[0]);
      str += VisitExpression ((dynamic) node[1]);
      var varName = node[0].AnchorToken.Lexeme;
      str += "\t\ti32.sub\n";
      if (TempLocalSymbolTable.Contains (varName)) {
        str += $"\t\tlocal.set ${varName}\n";
      } else {
        str += $"\t\tglobal.set ${varName}\n";
      }
      return str;
    }
    //-----------------------------------------------------------
    public string Visit (FunctionCall node, bool isExpression = false) {
      var str = VisitExpressions (node);
      var functionName = node.AnchorToken.Lexeme;
      if (GlobalFunctions[functionName].isPrimitive) {
        GlobalFunctions[functionName].name = functionName;
        apiUsed.Add (GlobalFunctions[functionName]);
      }
      str += $"\t\tcall ${functionName}\n";
      if (!isExpression) {
        str += "\t\tdrop\n";
      }
      return str;
    }
    //-----------------------------------------------------------
    public string Visit (If node) {
      //Lista de Condición - Statement
      List<Tuple<Node, Node>> ifs = new List<Tuple<Node, Node>> ();
      ifs.Add (new Tuple<Node, Node> (node[0], node[1]));
      foreach (var child in node[2]) {
        ifs.Add (new Tuple<Node, Node> (child[0], child[1]));
      }
      return IfHelper (ifs, (Else) node[3], 0);
    }
    string IfHelper (List<Tuple<Node, Node>> ifs, Else elseNode, int index) {
      if (ifs.Count == index) {
        return Visit (elseNode);
      }
      return VisitExpression ((dynamic) ifs[index].Item1) +
        $"{Ident(index+1)}if\n" +
        Visit ((dynamic) ifs[index].Item2) +
        $"{Ident(index+1)}else\n" +
        IfHelper (ifs, elseNode, index + 1) +
        $"{Ident(index+1)}end\n";
    }
    //-----------------------------------------------------------
    public string Visit (ElifList node) {
      //Allready Processed
      return "";
    }
    //-----------------------------------------------------------
    public string Visit (Elif node) {
      //Allready Processed
      return "";
    }
    //-----------------------------------------------------------
    public string Visit (Else node) {
      return VisitChildren (node);;
    }
    //-----------------------------------------------------------
    public string Visit (While node) {
      var label1 = Generatelabel ();
      var label2 = Generatelabel ();
      whileStack.Push (label1);
      var str = $"{Ident(whileStack.Count)}block " + label1 + "\n" +
        $"{Ident(whileStack.Count + 1)}loop " + label2 + "\n" +
        VisitExpression ((dynamic) node[0]) +
        $"{Ident(whileStack.Count + 1)}i32.eqz\n" +
        $"{Ident(whileStack.Count + 1)}br_if " + label1 + "\n" +
        Visit ((dynamic) node[1]) +
        $"{Ident(whileStack.Count+ 1)}br " + label2 + "\n" +
        $"{Ident(whileStack.Count +  1)}end\n" +
        $"{Ident(whileStack.Count)}end\n";
      whileStack.Pop();
      return str;
    }
    //-----------------------------------------------------------
    public string Visit (Break node) {
      return $"{Ident(whileStack.Count + 1)}br " + whileStack.Peek () + "\n";
    }
    //-----------------------------------------------------------
    public string Visit (Return node) {
      return VisitExpression (node[0]) +
        "\t\treturn\n";
    }
    //-----------------------------------------------------------
    public string Visit (EmptyStatement node) {
      //Is an empty statemnt dude
      return "\t\tnop\n";
    }

    #region Expressions
    public string Visit (ExpressionList node) {
      return VisitExpressions (node);
    }
    //-----------------------------------------------------------
    public string Visit (Or node) {
      return VisitExpression ((dynamic) node[0]) +
        "\t\tif (result i32)\n" +
        "\t\ti32.const 1\n" +
        "\t\telse\n" +
        VisitExpression ((dynamic) node[1]) +
        "\t\tend\n";
    }
    //-----------------------------------------------------------
    public string Visit (And node) {
      return VisitExpression ((dynamic) node[0]) +
        "\t\tif (result i32)\n" +
        VisitExpression ((dynamic) node[1]) +
        "\t\telse\n" +
        "\t\ti32.const 0\n" +
        "\t\tend\n";
    }
    //-----------------------------------------------------------
    public string Visit (EqualTo node) {
      return VisitBinaryOperator ("i32.eq", node);
    }
    //-----------------------------------------------------------
    public string Visit (NotEqualTo node) {
      return VisitBinaryOperator ("i32.eq", node) + "\t\ti32.eqz\n";
    }
    //-----------------------------------------------------------
    public string Visit (LessThan node) {
      return VisitBinaryOperator ("i32.lt_s", node);
    }
    //-----------------------------------------------------------
    public string Visit (LessEqualThan node) {
      return VisitBinaryOperator ("i32.le_s", node);
    }
    //-----------------------------------------------------------
    public string Visit (GreaterThan node) {
      return VisitBinaryOperator ("i32.gt_s", node);
    }
    //-----------------------------------------------------------
    public string Visit (GreaterEqualThan node) {
      return VisitBinaryOperator ("i32.ge_s", node);
    }
    //-----------------------------------------------------------
    public string Visit (Plus node) {
      return VisitBinaryOperator ("i32.add", node);
    }
    //-----------------------------------------------------------
    public string Visit (Subtraction node) {
      return VisitBinaryOperator ("i32.sub", node);
    }
    //-----------------------------------------------------------
    public string Visit (Neg node) {
      return "\t\ti32.const 0\n" + VisitExpression ((dynamic) node[0]) + "\t\ti32.sub\n";
    }
    //-----------------------------------------------------------
    public string Visit (Times node) {
      return VisitBinaryOperator ("i32.mul", node);
    }
    //-----------------------------------------------------------
    public string Visit (Divide node) {
      return VisitBinaryOperator ("i32.div_s", node);
    }
    //-----------------------------------------------------------
    public string Visit (Modulo node) {
      return VisitBinaryOperator ("i32.rem_s", node);
    }
    //-----------------------------------------------------------
    public string Visit (Not node) {
      return VisitExpression ((dynamic) node[0]) + "\t\ti32.eqz\n";
    }

    #endregion

    #region Literals
    //-----------------------------------------------------------
    public string Visit (Identifier node) {
      var varName = node.AnchorToken.Lexeme;
      if (TempLocalSymbolTable != null && TempLocalSymbolTable.Contains (varName)) {
        if (inVariableDefinition)
          return $"    (local ${varName} i32)\n";
      } else if (!inVariableDefinition && GlobalVariables.Contains (varName)) {
        //Found a GlobalVariable, but this logic was already processed
        return $"\t\tglobal.get ${varName}\n";
      } else if (GlobalVariables.Contains (varName)) {
        return "";
      }
      return $"\t\tlocal.get ${varName}\n";
    }
    //-----------------------------------------------------------
    public string Visit (IntLiteral node) {
      return $"    i32.const {node.AnchorToken.Lexeme}\n";
    }
    //-----------------------------------------------------------
    public string Visit (int node) {
      return $"    i32.const {node}\n";
    }
    //-----------------------------------------------------------
    public string Visit (True node) {
      return "    i32.const 1\n";
    }
    //-----------------------------------------------------------
    public string Visit (False node) {
      return "    i32.const 0\n";
    }
    //-----------------------------------------------------------
    public string Visit (CharLiteral node) {
      return $"    i32.const {AsCodePoint(node.AnchorToken.Lexeme)[0]}\n";
    }
    //-----------------------------------------------------------
    public string Visit (StringLiteral node) {
      var lexeme = node.AnchorToken.Lexeme; // "ABC\n"
      var lexemeArr = AsCodePoint (lexeme); //[65, 66, 67, 10];
      return ListToWat (lexemeArr);
    }

    public string Visit (Array node) {
      return ListToWat (node); //
    }

    string ListToWat (dynamic array, string tempName = null) {
      tempName = tempName ?? Generatelabel ();
      this.tempVariables.Add (tempName);
      AddListApi ();
      var sb =
        $"\t\ti32.const 0\n" +
        "\t\tcall $new\n" +
        $"\t\tlocal.set {tempName}\n";
      foreach (var item in array) {
        sb +=
          $"\t\tlocal.get {tempName}\n" +
          VisitExpression (item) +
          "\t\tcall $add\n" +
          "\t\tdrop\n";
      }
      sb += $"\t\tlocal.get {tempName}\n";
      return sb;
    }

    void AddListApi () {
      GlobalFunctions["add"].name = "add";
      GlobalFunctions["new"].name = "new";
      this.apiUsed.Add (GlobalFunctions["new"]);
      this.apiUsed.Add (GlobalFunctions["add"]);
    }

    #endregion
    //-----------------------------------------------------------
    string VisitChildren (Node node) {
      var sb = new StringBuilder ();
      foreach (var n in node) {
        sb.Append (Visit ((dynamic) n));
      }
      return sb.ToString ();
    }

    string VisitExpressions (Node node) {
      var sb = new StringBuilder ();
      foreach (var n in node) {
        sb.Append (VisitExpression ((dynamic) n));
      }
      return sb.ToString ();
    }
    //-----------------------------------------------------------
    string VisitBinaryOperator (string op, Node node) {
      return VisitExpression ((dynamic) node[0]) +
        VisitExpression ((dynamic) node[1]) +
        $"    {op}\n";
    }
    //-----------------------------------------------------------
    string Ident (int tabs) {
      return new String ('\t', tabs * 2);
    }
  }
}