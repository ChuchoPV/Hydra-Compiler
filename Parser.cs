/*
  Hydra compiler - Token class for the Parser.
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
  public class Parser {
    IEnumerator<Token> tokenStream;
    public Parser (IEnumerator<Token> tokenStream) {
      this.tokenStream = tokenStream;
      this.tokenStream.MoveNext ();
    }
    public TokenCategory Current {
      get { return tokenStream.Current.Category; }
    }
    public Token Expect (TokenCategory category) {
      if (Current == category) {
        Token current = tokenStream.Current;
        tokenStream.MoveNext ();
        return current;
      }
      //tokenStream.MoveNext();
      throw new SyntaxError (category, tokenStream.Current);
    }
    static readonly ISet<TokenCategory> firstOfDeclaration =
      new HashSet<TokenCategory> () {
        TokenCategory.VAR,
        TokenCategory.ID
      };
    static readonly ISet<TokenCategory> firstOfExpression =
      new HashSet<TokenCategory> () {
        TokenCategory.ID,
        TokenCategory.LIT_INT,
        TokenCategory.LIT_CHAR,
        TokenCategory.LIT_STR,
        TokenCategory.TRUE,
        TokenCategory.FALSE,
        TokenCategory.OPEN_PAR,
        TokenCategory.OPEN_BRAC,
        TokenCategory.NEG
      };
    static readonly ISet<TokenCategory> firstOfStatement =
      new HashSet<TokenCategory> () {
        TokenCategory.ID,
        TokenCategory.IF,
        TokenCategory.WHILE,
        TokenCategory.BREAK,
        TokenCategory.RETURN,
        TokenCategory.SEMICOLON,
        TokenCategory.VAR
      };

    static readonly ISet<TokenCategory> firstOfOpUnary =
      new HashSet<TokenCategory> () {
        TokenCategory.NEG,
        TokenCategory.NOT,
        TokenCategory.PLUS,
      };

    public Node Prog () {
      var prog = new Program ();
      prog.Add (DefList ());
      Expect (TokenCategory.EOF);
      return prog;
    }

    public Node DefList () {
      var defListNode = new DefinitionList ();
      while (firstOfDeclaration.Contains (Current)) {
        defListNode.Add (
          Def ()
        );
      }
      return defListNode;
    }

    public Node Def () {
      switch (Current) {
        case TokenCategory.VAR:
          return VarDef ();
        case TokenCategory.ID:
          return FunDef ();
        default:
          throw new SyntaxError (firstOfDeclaration, tokenStream.Current);
      }
    }

    public Node VarDef () {
      var vardeflist = new VariableDefinitionList ();
      Expect (TokenCategory.VAR);
      IdList (vardeflist);
      Expect (TokenCategory.SEMICOLON);
      return vardeflist;
    }
    public void IdList (Node vardeflist) {
      var identifier = new Identifier () {
        AnchorToken = Expect (TokenCategory.ID)
      };
      if (Current == TokenCategory.ASSIGN) {
        Expect (TokenCategory.ASSIGN);
        var assignment = new Assignment();
        assignment.Add(identifier);
        assignment.Add(Expr ());
        vardeflist.Add (assignment);
      }else{
        vardeflist.Add (identifier);
      }
      while (Current == TokenCategory.COMMA) {
        Expect (TokenCategory.COMMA);
        IdList (vardeflist);
      }
    }

    public Node FunDef () {
      var id = Expect (TokenCategory.ID);
      var fundef = new FunctionDefinition () {
        AnchorToken = id
      };

      Expect (TokenCategory.OPEN_PAR);
      var idParamList = new IdParameterList ();
      IdParamList (idParamList);
      fundef.Add (idParamList);
      Expect (TokenCategory.CLOSE_PAR);

      Expect (TokenCategory.OPEN_CURLY);
      fundef.Add (VarDefList ());
      fundef.Add (StmtList ());
      Expect (TokenCategory.CLOSE_CURLY);
      return fundef;
    }
    public void IdParamList (Node idParamList) {
      if (Current == TokenCategory.ID) {
        RestIDParamList (idParamList);
      }
    }
    public void RestIDParamList (Node idParamList) {
      var id = Expect (TokenCategory.ID);
      idParamList.Add (new Identifier () {
        AnchorToken = id
      });
      while (Current == TokenCategory.COMMA) {
        Expect (TokenCategory.COMMA);
        RestIDParamList (idParamList);
      }
    }
    public Node VarDefList () {
      Node vardeflist = new VariableDefinitionList ();
      while (Current == TokenCategory.VAR) {
        vardeflist = VarDef ();
      }
      return vardeflist;
    }

    public Node StmtList () {
      var statementLis = new StatementList ();
      while (firstOfStatement.Contains (Current)) {
        statementLis.Add (Stmt ());
      }
      return statementLis;
    }
    public Node Stmt () {
      switch (Current) {
        case TokenCategory.ID:
          var stmt = StmtID ();
          Expect (TokenCategory.SEMICOLON);
          return stmt;
        case TokenCategory.IF:
          return If ();
        case TokenCategory.WHILE:
          return While ();
        case TokenCategory.BREAK:
          var breakNode = new Break () {
            AnchorToken = Expect (TokenCategory.BREAK)
          };
          Expect (TokenCategory.SEMICOLON);
          return breakNode;
        case TokenCategory.RETURN:
          Expect (TokenCategory.RETURN);
          var returnNode = new Return ();
          returnNode.Add (Expr ());
          Expect (TokenCategory.SEMICOLON);
          return returnNode;
        case TokenCategory.SEMICOLON:
          var empty = new EmptyStatement ();
          Expect (TokenCategory.SEMICOLON);
          return empty;
        case TokenCategory.VAR:
          return VarDefList ();
        default:
          throw new SyntaxError (firstOfStatement, tokenStream.Current);
      }
    }
    public Node StmtID () {
      var id = Expect (TokenCategory.ID);
      switch (Current) {
        case TokenCategory.ASSIGN:
          Expect (TokenCategory.ASSIGN);
          var expr = new Assignment ();
          expr.Add(new Identifier(){
            AnchorToken = id
          });
          expr.Add (Expr ());
          return expr;
        case TokenCategory.PLUSPLUS:
          Expect (TokenCategory.PLUSPLUS);
          var plusplus = new PlusPlus ();
          plusplus.Add (new Identifier () {
            AnchorToken = id
          });
          return plusplus;
        case TokenCategory.LESSLESS:
          Expect (TokenCategory.LESSLESS);
          var lessless = new LesLess ();
          lessless.Add (new Identifier () {
            AnchorToken = id
          });
          return lessless;
        case TokenCategory.PLUSEQUAL:
          Expect (TokenCategory.PLUSEQUAL);
          var plusequal = new PlusEqual ();
          plusequal.Add (new Identifier () {
            AnchorToken = id
          });
          plusequal.Add (Expr ());
          return plusequal;
        case TokenCategory.SUBTRACEQUAL:
          Expect (TokenCategory.SUBTRACEQUAL);
          var subcequal = new SubtracEqual ();
          subcequal.Add (new Identifier () {
            AnchorToken = id
          });
          subcequal.Add (Expr ());
          return subcequal;
        case TokenCategory.OPEN_PAR:
          return FunCall (id);
        default:
          throw new SyntaxError (firstOfDeclaration, tokenStream.Current);
      }
    }
    public Node FunCall (Token id) {
      var functionCall = new FunctionCall (){
        AnchorToken = id
      };
      Expect (TokenCategory.OPEN_PAR);
      functionCall.Add(ExprList ());
      Expect (TokenCategory.CLOSE_PAR);
      return functionCall;
    }
    public Node ExprList () {
      var expressionList = new ExpressionList();
      if (firstOfExpression.Contains (Current)) {
        expressionList.Add(Expr ());
        ExprListCont (expressionList);
      }
      return expressionList;
    }
    public void ExprListCont (Node expressionList) {
      while (Current == TokenCategory.COMMA) {
        Expect (TokenCategory.COMMA);
        expressionList.Add(Expr ());
        ExprListCont (expressionList);
      }
    }
    public Node If () {
      var ifNode = new If ();
      Expect (TokenCategory.IF);
      Expect (TokenCategory.OPEN_PAR);
      ifNode.Add (Expr ());
      Expect (TokenCategory.CLOSE_PAR);
      Expect (TokenCategory.OPEN_CURLY);
      ifNode.Add (StmtList ());
      Expect (TokenCategory.CLOSE_CURLY);
      ifNode.Add (ElseIfList ());
      ifNode.Add (Else ());
      return ifNode;
    }
    public Node ElseIfList () {
      var eliflist = new ElifList ();
      while (Current == TokenCategory.ELIF) {
        var elif = new Elif ();
        Expect (TokenCategory.ELIF);
        Expect (TokenCategory.OPEN_PAR);
        elif.Add (Expr ());
        Expect (TokenCategory.CLOSE_PAR);
        Expect (TokenCategory.OPEN_CURLY);
        elif.Add (StmtList ());
        Expect (TokenCategory.CLOSE_CURLY);
        eliflist.Add (elif);
      }
      return eliflist;
    }
    public Node Else () {
      var elseNode = new Else ();
      if (Current == TokenCategory.ELSE) {
        Expect (TokenCategory.ELSE);
        Expect (TokenCategory.OPEN_CURLY);
        elseNode.Add (StmtList ());
        Expect (TokenCategory.CLOSE_CURLY);
      }
      return elseNode;
    }
    public Node While () {
      Expect (TokenCategory.WHILE);
      Expect (TokenCategory.OPEN_PAR);
      var whileNode = new While ();
      whileNode.Add (Expr ());
      Expect (TokenCategory.CLOSE_PAR);
      Expect (TokenCategory.OPEN_CURLY);
      whileNode.Add (StmtList ());
      Expect (TokenCategory.CLOSE_CURLY);
      return whileNode;
    }

    public Node Expr () {
      var expression = new Expression();
      expression.Add(ExprOr());
      return expression;
    }
    public Node ExprOr () {
      Node expr = ExprAnd();
      while (Current == TokenCategory.OR) {
        Expect (TokenCategory.OR);
        var exprOr = new ExpressionOr();
        exprOr.Add(expr);
        exprOr.Add(ExprAnd ());
        expr = exprOr;
      }
      return expr;
    }
    public Node ExprAnd () {
      Node expr = ExprComp();
      while (Current == TokenCategory.AND) {
        Expect (TokenCategory.AND);
        var exprAnd = new ExpressionAnd();
        exprAnd.Add(expr);
        exprAnd.Add(ExprComp ());
        expr = exprAnd;
      }
      return expr;
    }
    public Node ExprComp () {
      Node expr = ExprRel();
      Node expr2;
      while (Current == TokenCategory.EQUALTO || Current == TokenCategory.NOTEQUALTO) {
        switch (Current) {
          case TokenCategory.EQUALTO:
            Expect (TokenCategory.EQUALTO);
            expr2 = new ExpressionEqualTo();
            break;
          case TokenCategory.NOTEQUALTO:
            Expect (TokenCategory.NOTEQUALTO);
            expr2 = new ExpressionNotEqualTo();
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () { TokenCategory.EQUALTO, TokenCategory.NOTEQUALTO }, tokenStream.Current);
        }
        expr2.Add(expr);
        expr2.Add(ExprRel ());
        expr = expr2;
      }
      return expr;
    }
    public Node ExprRel () {
      Node expr = ExprAdd ();
      Node expr2;
      while (Current == TokenCategory.LESS || Current == TokenCategory.LESSEQUAL ||
        Current == TokenCategory.GREATER || Current == TokenCategory.GREATEREQUAL
      ) {
        switch (Current) {
          case TokenCategory.LESS:
            Expect (TokenCategory.LESS);
            expr2 = new ExpressionLess();
            break;
          case TokenCategory.LESSEQUAL:
            Expect (TokenCategory.LESSEQUAL);
            expr2 = new ExpressionLessEqual();
            break;
          case TokenCategory.GREATER:
            Expect (TokenCategory.GREATER);
            expr2 = new ExpressionGreater();
            break;
          case TokenCategory.GREATEREQUAL:
            Expect (TokenCategory.GREATEREQUAL);
            expr2 = new ExpressionGreaterEqual();
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () {
              TokenCategory.LESS, TokenCategory.LESSEQUAL,
                TokenCategory.GREATER, TokenCategory.GREATEREQUAL
            }, tokenStream.Current);
        }
        expr2.Add(expr);
        expr2.Add(ExprAdd ());
        expr = expr2;
      }
      return expr;
    }
    public Node ExprAdd () {
      Node expr = ExprMul ();
      Node expr2;
      while (Current == TokenCategory.PLUS || Current == TokenCategory.NEG) {
        switch (Current) {
          case TokenCategory.PLUS:
            Expect (TokenCategory.PLUS);
            expr2 = new Plus();
            break;
          case TokenCategory.NEG:
            Expect (TokenCategory.NEG);
            expr2 = new Less();
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () {
              TokenCategory.PLUS, TokenCategory.NEG,
            }, tokenStream.Current);
        }
        expr2.Add(expr);
        expr2.Add(ExprMul ());
        expr = expr2;
      }
      return expr;
    }
    public Node ExprMul () {
      Node expr = ExprUnary ();
      Node expr2;
      while (Current == TokenCategory.TIMES || Current == TokenCategory.DIV ||
        Current == TokenCategory.MOD
      ) {
        switch (Current) {
          case TokenCategory.TIMES:
            Expect (TokenCategory.TIMES);
            expr2 = new Times();
            break;
          case TokenCategory.DIV:
            Expect (TokenCategory.DIV);
            expr2 = new Divide();
            break;
          case TokenCategory.MOD:
            Expect (TokenCategory.MOD);
            expr2 = new Modulo();
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () {
              TokenCategory.TIMES, TokenCategory.DIV,
                TokenCategory.MOD
            }, tokenStream.Current);
        }
        expr2.Add(expr);
        expr2.Add(ExprUnary ());
        expr = expr2;
      }
      return expr;
    }
    public Node ExprUnary () {
      switch (Current) {
        case TokenCategory.PLUS:
        case TokenCategory.NEG:
        case TokenCategory.NOT:
          var opunary = new UnaryOperation();
          opunary.Add(OpUnary ());
          opunary.Add(ExprUnary ());
          return opunary;
        case TokenCategory.ID:
        case TokenCategory.LIT_INT:
        case TokenCategory.TRUE:
        case TokenCategory.FALSE:
        case TokenCategory.LIT_CHAR:
        case TokenCategory.LIT_STR:
        case TokenCategory.OPEN_BRAC:
        case TokenCategory.OPEN_PAR:
          return ExprPrimary ();
        default:
          ISet<TokenCategory> set = firstOfExpression;
          set.UnionWith (firstOfOpUnary);
          throw new SyntaxError (set, tokenStream.Current);
      }
    }
    public Node OpUnary () {
      switch (Current) {
        case TokenCategory.PLUS:
          return new Plus(){
            AnchorToken = Expect (TokenCategory.PLUS)
          };
        case TokenCategory.NEG:
          return new Neg(){
            AnchorToken = Expect (TokenCategory.NEG)
          };
        case TokenCategory.NOT:
          return new Not(){
            AnchorToken = Expect (TokenCategory.NOT)
          };
        default:
          throw new SyntaxError (firstOfOpUnary, tokenStream.Current);
      }
    }
    public Node ExprPrimary () {
      switch (Current) {
        case TokenCategory.ID:
          var id = Expect (TokenCategory.ID);
          Node expr = new Identifier(){
            AnchorToken = id
          };
          if (Current == TokenCategory.OPEN_PAR) {
            expr = FunCall (id);
          }
          return expr;
        case TokenCategory.LIT_INT:
          return new IntLiteral () {
            AnchorToken = Expect (TokenCategory.LIT_INT)
          };
        case TokenCategory.TRUE:
          return new True () {
            AnchorToken = Expect (TokenCategory.TRUE)
          };
        case TokenCategory.FALSE:
          return new False () {
            AnchorToken = Expect (TokenCategory.FALSE)
          };
        case TokenCategory.LIT_CHAR:
          return new CharLiteral () {
            AnchorToken = Expect (TokenCategory.LIT_CHAR)
          };
        case TokenCategory.LIT_STR:
          return new StringLiteral () {
            AnchorToken = Expect (TokenCategory.LIT_STR)
          };
        case TokenCategory.OPEN_BRAC:
          return Array ();
        case TokenCategory.OPEN_PAR:
          Expect (TokenCategory.OPEN_PAR);
          var expression = Expr ();
          Expect (TokenCategory.CLOSE_PAR);
          return expression;
        default:
          throw new SyntaxError (firstOfExpression, tokenStream.Current);
      }
    }
    public Node Array () {
      var array = new Array ();
      Expect (TokenCategory.OPEN_BRAC);
      ExprList ();
      Expect (TokenCategory.CLOSE_BRAC);
      return array;
    }

  }
}