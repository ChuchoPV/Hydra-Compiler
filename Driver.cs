/*
  Hydra compiler - Program driver.
  Copyright (C) 2013-2020 Ariel Ortiz, ITESM CEM
  modified by: Jesús Perea, Jorge López, Gerardo Galván.

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
using System.IO;

namespace Hydra_compiler {
  public class Driver {
    const string VERSION = "0.5";

    //-----------------------------------------------------------
    static readonly string[] ReleaseIncludes = {
      "Lexical analysis",
      "Syntactic analysis",
      "AST construction",
      "Semantic analysis",
      "WAT code generation"
    };

    //-----------------------------------------------------------
    void PrintAppHeader () {
      Console.WriteLine ("Hydra compiler, version " + VERSION);
      Console.WriteLine ("Copyright \u00A9 2020-2020 Ariel Ortiz, ITESM CEM\nmodified by: Jesús Perea, Jorge López, Gerardo Galván.");
      Console.WriteLine ("This program is free software; you may " +
        "redistribute it under the terms of");
      Console.WriteLine ("the GNU General Public License version 3 or " +
        "later.");
      Console.WriteLine ("This program has absolutely no warranty.");
    }

    //-----------------------------------------------------------
    void PrintReleaseIncludes () {
      Console.WriteLine ("Included in this release:");
      foreach (var phase in ReleaseIncludes) {
        Console.WriteLine ("   * " + phase);
      }
    }

    void SetAPI (SemanticAnalyzer semantic) {
      semantic.GlobalFunctions["printi"] = new FunctionData () {
        arity = 1,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["printc"] = new FunctionData () {
        arity = 1,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["prints"] = new FunctionData () {
        arity = 1,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["println"] = new FunctionData () {
        arity = 0,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["readi"] = new FunctionData () {
        arity = 0,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["reads"] = new FunctionData () {
        arity = 0,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["new"] = new FunctionData () {
        arity = 1,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["size"] = new FunctionData () {
        arity = 1,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["add"] = new FunctionData () {
        arity = 2,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["get"] = new FunctionData () {
        arity = 2,
        isPrimitive = true,
        RefST = null
      };
      semantic.GlobalFunctions["set"] = new FunctionData () {
        arity = 3,
        isPrimitive = true,
        RefST = null
      };

    }
    //-----------------------------------------------------------
    void Run (string[] args) {

      PrintAppHeader ();
      Console.WriteLine ();
      PrintReleaseIncludes ();
      Console.WriteLine ();

      if (args.Length != 2) {
        Console.Error.WriteLine (
          "Please specify the name of the input file.");
        Environment.Exit (1);
      }

      if (args.Length == 2) {
        try {
          var inputPath = args[0];
          var outputPath = args[1];
          var input = File.ReadAllText (inputPath);
          var parser = new Parser (new Scanner (input).Start ().GetEnumerator ());
          var ast = parser.Prog ();
          // Console.WriteLine(ast.ToStringTree());
          var semantic = new SemanticAnalyzer ();
          SetAPI (semantic);
          semantic.isFirstPass = true;
          semantic.Visit ((dynamic) ast);
          semantic.isFirstPass = false;
          semantic.Visit ((dynamic) ast);

          if (!semantic.GlobalFunctions.Contains ("main")) {
            throw new SemanticError (
              "The main function was not found",
              new FileInfo (inputPath).FullName
            );
          }
          Console.WriteLine ("Semantics OK.");
          var codeGenerator = new WATVisitor (semantic.GlobalVariables, semantic.GlobalFunctions);
          File.WriteAllText (
            outputPath,
            codeGenerator.Visit ((dynamic) ast));
          Console.WriteLine (
            "Created WAT (WebAssembly text format) file " +
            $"'{outputPath}'.");

        } catch (Exception e) {
          if (e is FileNotFoundException || e is SyntaxError || e is SemanticError) {
            Console.Error.WriteLine (e.Message);
            Environment.Exit (1);
          }

          throw;
        }
      } else {
        try {
          Console.Write ("> ");
          // var input = Console.ReadLine ();
          var outputPath = "output.wat";
          var input = File.ReadAllText ("test.hydra");
          var parser = new Parser (new Scanner (input).Start ().GetEnumerator ());
          var ast = parser.Prog ();
          // Console.WriteLine (ast.ToStringTree ());
          // return;
          var semantic = new SemanticAnalyzer ();
          SetAPI (semantic);
          semantic.isFirstPass = true;
          semantic.Visit ((dynamic) ast);
          semantic.isFirstPass = false;
          semantic.Visit ((dynamic) ast);

          if (!semantic.GlobalFunctions.Contains ("main")) {
            throw new SemanticError (
              "The main function was not found",
              new FileInfo ("code_samples/000_test.hydra").FullName
            );
          }
          Console.WriteLine ("Semantics OK.");
          var codeGenerator = new WATVisitor (semantic.GlobalVariables, semantic.GlobalFunctions);
          File.WriteAllText (
            outputPath,
            codeGenerator.Visit ((dynamic) ast));
          Console.WriteLine (
            "Created WAT (WebAssembly text format) file " +
            $"'{outputPath}'.");

        } catch (Exception e) {
          if (e is FileNotFoundException || e is SyntaxError || e is SemanticError) {
            Console.Error.WriteLine (e.Message);
            Environment.Exit (1);
          }

          throw;
        }
      }
    }

    //-----------------------------------------------------------
    public static void Main (string[] args) {
      new Driver ().Run (args);
    }
  }
}