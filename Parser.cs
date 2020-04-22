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

    public void Prog () {
      while (firstOfDeclaration.Contains (Current)) {
        Def ();
      }
      Expect (TokenCategory.EOF);
    }

    public void Def () {
      switch (Current) {
        case TokenCategory.VAR:
          VarDef ();
          break;
        case TokenCategory.ID:
          FunDef ();
          break;
        default:
          throw new SyntaxError (firstOfDeclaration, tokenStream.Current);
      }
    }

    public void VarDef () {
      Expect (TokenCategory.VAR);
      IdList ();
      Expect (TokenCategory.SEMICOLON);
    }
    public void IdList () {
      Expect (TokenCategory.ID);
      while (Current == TokenCategory.ASSIGN) {
        Expect (TokenCategory.ASSIGN);
        Expr ();
      }
      while (Current == TokenCategory.COMMA) {
        Expect (TokenCategory.COMMA);
        IdList ();
      }
    }

    public void FunDef () {
      Expect (TokenCategory.ID);
      Expect (TokenCategory.OPEN_PAR);
      IdParamList ();
      Expect (TokenCategory.CLOSE_PAR);
      Expect (TokenCategory.OPEN_CURLY);
      VarDefList ();
      StmtList ();
      Expect (TokenCategory.CLOSE_CURLY);

    }
    public void IdParamList () {
      if (Current == TokenCategory.ID) {
        Expect (TokenCategory.ID);
        while (Current == TokenCategory.COMMA) {
          Expect (TokenCategory.COMMA);
          IdParamList ();
        }
      }
    }
    public void VarDefList () {
      while (Current == TokenCategory.VAR) {
        VarDef ();
      }
    }

    public void StmtList () {
      while (firstOfStatement.Contains (Current)) {
        Stmt ();
      }
    }
    public void Stmt () {
      switch (Current) {
        case TokenCategory.ID:
          StmtID ();
          Expect (TokenCategory.SEMICOLON);
          break;
        case TokenCategory.IF:
          If ();
          break;
        case TokenCategory.WHILE:
          While ();
          break;
        case TokenCategory.BREAK:
          Expect (TokenCategory.BREAK);
          Expect (TokenCategory.SEMICOLON);
          break;
        case TokenCategory.RETURN:
          Expect (TokenCategory.RETURN);
          Expr ();
          Expect (TokenCategory.SEMICOLON);
          break;
        case TokenCategory.SEMICOLON:
          Expect (TokenCategory.SEMICOLON);
          break;
        case TokenCategory.VAR:
          VarDefList ();
          break;
        default:
          throw new SyntaxError (firstOfStatement, tokenStream.Current);
      }
    }
    public void StmtID () {
      Expect (TokenCategory.ID);
      switch (Current) {
        case TokenCategory.ASSIGN:
          Expect (TokenCategory.ASSIGN);
          Expr ();
          break;
        case TokenCategory.PLUSPLUS:
          Expect (TokenCategory.PLUSPLUS);
          break;
        case TokenCategory.LESSLESS:
          Expect (TokenCategory.LESSLESS);
          break;
        case TokenCategory.PLUSEQUAL:
          Expect (TokenCategory.PLUSEQUAL);
          Expr ();
          break;
        case TokenCategory.SUBTRACEQUAL:
          Expect (TokenCategory.SUBTRACEQUAL);
          Expr ();
          break;
        case TokenCategory.OPEN_PAR:
          FunCall ();
          break;
        default:
          throw new SyntaxError (firstOfDeclaration, tokenStream.Current);
      }
    }
    public void FunCall () {
      Expect (TokenCategory.OPEN_PAR);
      ExprList ();
      Expect (TokenCategory.CLOSE_PAR);
    }
    public void ExprList () {
      if (firstOfExpression.Contains (Current)) {
        Expr ();
        ExprListCont ();
      }
    }
    public void ExprListCont () {
      while (Current == TokenCategory.COMMA) {
        Expect (TokenCategory.COMMA);
        Expr ();
        ExprList ();
      }
    }
    public void If () {
      Expect (TokenCategory.IF);
      Expect (TokenCategory.OPEN_PAR);
      Expr ();
      Expect (TokenCategory.CLOSE_PAR);
      Expect (TokenCategory.OPEN_CURLY);
      StmtList ();
      Expect (TokenCategory.CLOSE_CURLY);
      ElseIfList ();
      Else ();
    }
    public void ElseIfList () {
      while (Current == TokenCategory.ELIF) {
        Expect (TokenCategory.ELIF);
        Expect (TokenCategory.OPEN_PAR);
        Expr ();
        Expect (TokenCategory.CLOSE_PAR);
        Expect (TokenCategory.OPEN_CURLY);
        StmtList ();
        Expect (TokenCategory.CLOSE_CURLY);
      }
    }
    public void Else () {
      if (Current == TokenCategory.ELSE) {
        Expect (TokenCategory.ELSE);
        Expect (TokenCategory.OPEN_CURLY);
        StmtList ();
        Expect (TokenCategory.CLOSE_CURLY);
      }
    }
    public void While () {
      Expect (TokenCategory.WHILE);
      Expect (TokenCategory.OPEN_PAR);
      Expr ();
      Expect (TokenCategory.CLOSE_PAR);
      Expect (TokenCategory.OPEN_CURLY);
      StmtList ();
      Expect (TokenCategory.CLOSE_CURLY);
    }

    public void Expr () {
      ExprOr ();
    }
    public void ExprOr () {
      ExprAnd ();
      while (Current == TokenCategory.OR) {
        Expect (TokenCategory.OR);
        ExprAnd ();
      }
    }
    public void ExprAnd () {
      ExprComp ();
      while (Current == TokenCategory.AND) {
        Expect (TokenCategory.AND);
        ExprComp ();
      }
    }
    public void ExprComp () {
      ExprRel ();
      while (Current == TokenCategory.EQUALTO || Current == TokenCategory.NOTEQUALTO) {
        switch (Current) {
          case TokenCategory.EQUALTO:
            Expect (TokenCategory.EQUALTO);
            break;
          case TokenCategory.NOTEQUALTO:
            Expect (TokenCategory.NOTEQUALTO);
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () { TokenCategory.EQUALTO, TokenCategory.NOTEQUALTO }, tokenStream.Current);
        }
        ExprRel ();
      }
    }
    public void ExprRel () {
      ExprAdd ();
      while (Current == TokenCategory.LESS || Current == TokenCategory.LESSEQUAL ||
        Current == TokenCategory.GREATER || Current == TokenCategory.GREATEREQUAL
      ) {
        switch (Current) {
          case TokenCategory.LESS:
            Expect (TokenCategory.LESS);
            break;
          case TokenCategory.LESSEQUAL:
            Expect (TokenCategory.LESSEQUAL);
            break;
          case TokenCategory.GREATER:
            Expect (TokenCategory.GREATER);
            break;
          case TokenCategory.GREATEREQUAL:
            Expect (TokenCategory.GREATEREQUAL);
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () {
              TokenCategory.LESS, TokenCategory.LESSEQUAL,
                TokenCategory.GREATER, TokenCategory.GREATEREQUAL
            }, tokenStream.Current);
        }
        ExprAdd ();
      }
    }
    public void ExprAdd () {
      ExprMul ();
      while (Current == TokenCategory.PLUS || Current == TokenCategory.NEG) {
        switch (Current) {
          case TokenCategory.PLUS:
            Expect (TokenCategory.PLUS);
            break;
          case TokenCategory.NEG:
            Expect (TokenCategory.NEG);
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () {
              TokenCategory.PLUS, TokenCategory.NEG,
            }, tokenStream.Current);
        }
        ExprMul ();
      }
    }
    public void ExprMul () {
      ExprUnary ();
      while (Current == TokenCategory.TIMES || Current == TokenCategory.DIV ||
        Current == TokenCategory.MOD
      ) {
        switch (Current) {
          case TokenCategory.TIMES:
            Expect (TokenCategory.TIMES);
            break;
          case TokenCategory.DIV:
            Expect (TokenCategory.DIV);
            break;
          case TokenCategory.MOD:
            Expect (TokenCategory.MOD);
            break;
          default:
            throw new SyntaxError (new HashSet<TokenCategory> () {
              TokenCategory.TIMES, TokenCategory.DIV,
                TokenCategory.MOD
            }, tokenStream.Current);
        }
        ExprUnary ();
      }
    }
    public void ExprUnary () {
      switch (Current) {
        case TokenCategory.PLUS:
        case TokenCategory.NEG:
        case TokenCategory.NOT:
          OpUnary ();
          ExprUnary ();
          break;
        case TokenCategory.ID:
        case TokenCategory.LIT_INT:
        case TokenCategory.TRUE:
        case TokenCategory.FALSE:
        case TokenCategory.LIT_CHAR:
        case TokenCategory.LIT_STR:
        case TokenCategory.OPEN_BRAC:
        case TokenCategory.OPEN_PAR:
          ExprPrimary ();
          break;
        default:
          ISet<TokenCategory> set = firstOfExpression;
          set.UnionWith(firstOfOpUnary);
          throw new SyntaxError (set, tokenStream.Current);
      }
    }
    public void OpUnary () {
      switch (Current) {
        case TokenCategory.PLUS:
          Expect (TokenCategory.PLUS);
          break;
        case TokenCategory.NEG:
          Expect (TokenCategory.NEG);
          break;
        case TokenCategory.NOT:
          Expect (TokenCategory.NOT);
          break;
        default:
          throw new SyntaxError (firstOfOpUnary, tokenStream.Current);
      }
    }
    public void ExprPrimary () {
      switch (Current) {
        case TokenCategory.ID:
          Expect (TokenCategory.ID);
          if (Current == TokenCategory.OPEN_PAR) {
            FunCall ();
          }
          break;
        case TokenCategory.LIT_INT:
          Expect (TokenCategory.LIT_INT);
          break;
        case TokenCategory.TRUE:
          Expect (TokenCategory.TRUE);
          break;
        case TokenCategory.FALSE:
          Expect (TokenCategory.FALSE);
          break;
        case TokenCategory.LIT_CHAR:
          Expect (TokenCategory.LIT_CHAR);
          break;
        case TokenCategory.LIT_STR:
          Expect (TokenCategory.LIT_STR);
          break;
        case TokenCategory.OPEN_BRAC:
          Array ();
          break;
        case TokenCategory.OPEN_PAR:
          Expect (TokenCategory.OPEN_PAR);
          Expr ();
          Expect (TokenCategory.CLOSE_PAR);
          break;
        default:
          throw new SyntaxError (firstOfExpression, tokenStream.Current);
      }
    }
    public void Array () {
      Expect (TokenCategory.OPEN_BRAC);
      ExprList ();
      Expect (TokenCategory.CLOSE_BRAC);
    }

  }
}
