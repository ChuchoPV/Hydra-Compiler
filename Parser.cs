using System;
using System.Collections.Generic;

namespace Hydra_compiler
{
    public class Parser
    {
        IEnumerator<Token> tokenStream;
        public Parser(IEnumerator<Token> tokenStream)
        {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }
        public TokenCategory Current
        {
            get { return tokenStream.Current.Category; }
        }
        public Token Expect(TokenCategory category)
        {
            if (Current == category)
            {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            }
            //tokenStream.MoveNext();
            throw new SyntaxError(category, tokenStream.Current);
        }
        static readonly ISet<TokenCategory> firstOfDeclaration =
          new HashSet<TokenCategory>() {
        TokenCategory.VAR,
        TokenCategory.ID
          };
        static readonly ISet<TokenCategory> firstOfExpression =
          new HashSet<TokenCategory>() {
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
          new HashSet<TokenCategory>() {
        TokenCategory.ID,
        TokenCategory.IF,
        TokenCategory.WHILE,
        TokenCategory.BREAK,
        TokenCategory.RETURN,
        TokenCategory.SEMICOLON,
        TokenCategory.VAR
          };

        static readonly ISet<TokenCategory> firstOfOpUnary =
          new HashSet<TokenCategory>() {
        TokenCategory.NEG,
        TokenCategory.NOT,
        TokenCategory.PLUS,
          };

        public Node Prog()
        {
            Node prog = new Program();
            while (firstOfDeclaration.Contains(Current))
            {
                prog.Add(Def());
            }
            Expect(TokenCategory.EOF);
            return prog;
        }

        public Node Def()
        {
            switch (Current)
            {
                case TokenCategory.VAR:
                    return VarDef();
                case TokenCategory.ID:
                    return FunDef();
                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }

        public Node VarDef()
        {
            Node varDef = new VarDef() { AnchorToken = Expect(TokenCategory.VAR) };
            IdList(varDef);
            Expect(TokenCategory.SEMICOLON);
            return varDef;
        }
        public void IdList(Node varDef)
        {
            Node identifier = new Identifier() { AnchorToken = Expect(TokenCategory.ID) };
            if (Current == TokenCategory.ASSIGN)
            {
                Node assign = new Assign() { AnchorToken = Expect(TokenCategory.ASSIGN) };

                assign.Add(identifier);
                assign.Add(Expr());

                varDef.Add(assign);
            }
            else
            {
                varDef.Add(identifier);
            }

            while (Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                IdList(varDef);
            }
        }

        public Node FunDef()
        {
            Node funDef = new FunDef() { AnchorToken = Expect(TokenCategory.ID) };
            Expect(TokenCategory.OPEN_PAR);
            funDef.Add(IdParamList());
            Expect(TokenCategory.CLOSE_PAR);
            Expect(TokenCategory.OPEN_CURLY);
            funDef.Add(VarDefList());
            funDef.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY);
            return funDef;

        }

        public Node IdParamList(Node prevLst = null)
        {
            Node lst = prevLst ?? new IdParamList();
            if (Current == TokenCategory.ID)
            {
                lst.Add(new Identifier() { AnchorToken = Expect(TokenCategory.ID) });
                while (Current == TokenCategory.COMMA)
                {
                    Expect(TokenCategory.COMMA);
                    IdParamList(lst);
                }
            }
            return lst;
        }
        public Node VarDefList()
        {
            Node lst = new VarDefList();
            while (Current == TokenCategory.VAR)
            {
                lst.Add(VarDef());
            }
            return lst;
        }

        public Node StmtList()
        {
            Node lst = new StmtList();
            while (firstOfStatement.Contains(Current))
            {
                lst.Add(Stmt());
            }
            return lst;
        }
        public Node Stmt()
        {
            switch (Current)
            {
                case TokenCategory.ID:
                    Node stmt = StmtID();
                    Expect(TokenCategory.SEMICOLON);
                    return stmt;
                case TokenCategory.IF:
                    return If();
                case TokenCategory.WHILE:
                    return While();
                case TokenCategory.BREAK:
                    Node _break = new Break() { AnchorToken = Expect(TokenCategory.BREAK) };
                    Expect(TokenCategory.SEMICOLON);
                    return _break;
                case TokenCategory.RETURN:
                    Node _return = new Return() { AnchorToken = Expect(TokenCategory.RETURN) };
                    _return.Add(Expr());
                    Expect(TokenCategory.SEMICOLON);
                    return _return;
                case TokenCategory.SEMICOLON:
                    return new Empty() { AnchorToken = Expect(TokenCategory.SEMICOLON) };
                case TokenCategory.VAR:
                    return VarDefList();
                default:
                    throw new SyntaxError(firstOfStatement, tokenStream.Current);
            }
        }
        public Node StmtID()
        {
            Node identifier = new Identifier() { AnchorToken = Expect(TokenCategory.ID) };
            switch (Current)
            {
                case TokenCategory.PLUSPLUS:
                    Node op_pp = new PlusPlus() { AnchorToken = Expect(TokenCategory.PLUSPLUS) };
                    op_pp.Add(identifier);
                    return op_pp;
                case TokenCategory.LESSLESS:
                    Node op_ll = new LessLess() { AnchorToken = Expect(TokenCategory.LESSLESS) };
                    op_ll.Add(identifier);
                    return op_ll;
                case TokenCategory.PLUSEQUAL:
                    Node op_pe = new Increment() { AnchorToken = Expect(TokenCategory.PLUSEQUAL) };
                    op_pe.Add(identifier);
                    op_pe.Add(Expr());
                    return op_pe;
                case TokenCategory.SUBTRACEQUAL:
                    Node op_se = new Decrement() { AnchorToken = Expect(TokenCategory.SUBTRACEQUAL) };
                    op_se.Add(identifier);
                    op_se.Add(Expr());
                    return op_se;
                case TokenCategory.ASSIGN:
                    Node op_as = new Assign() { AnchorToken = Expect(TokenCategory.ASSIGN) };
                    op_as.Add(identifier);
                    op_as.Add(Expr());
                    return op_as;
                case TokenCategory.OPEN_PAR:
                    return FunCall(identifier.AnchorToken);
                default:
                    throw new SyntaxError(firstOfDeclaration, tokenStream.Current);
            }
        }
        public Node FunCall(Token identifier)
        {
            Node funCall = new FunCall() { AnchorToken = identifier };

            Token openPar = Expect(TokenCategory.OPEN_PAR);

            Node paramList = ExprList();
            // paramList.AnchorToken = openPar;

            Expect(TokenCategory.CLOSE_PAR);

            funCall.Add(paramList);
            return funCall;
        }

        public Node ExprList(Node prevLst = null)
        {
            Node lst = prevLst ?? new ExprList();
            if (firstOfExpression.Contains(Current))
            {
                lst.Add(Expr());
                ExprListCont(lst);
            }
            return lst;
        }
        public void ExprListCont(Node lst)
        {
            while (Current == TokenCategory.COMMA)
            {
                Expect(TokenCategory.COMMA);
                lst.Add(Expr());
                ExprList(lst);
            }
        }
        public Node If()
        {
            Node ifNode = new If();
            Expect(TokenCategory.IF);
            Expect(TokenCategory.OPEN_PAR);
            ifNode.Add(Expr());
            Expect(TokenCategory.CLOSE_PAR);
            Expect(TokenCategory.OPEN_CURLY);
            ifNode.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY);
            ifNode.Add(ElseIfList());
            ifNode.Add(Else());
            return ifNode;
        }
        public Node ElseIfList()
        {
            Node elifList = new ElseIfList();
            while (Current == TokenCategory.ELIF)
            {
                Node elifNode = new ElseIfNode();
                Expect(TokenCategory.ELIF);
                Expect(TokenCategory.OPEN_PAR);

                elifNode.Add(Expr());

                Expect(TokenCategory.CLOSE_PAR);
                Expect(TokenCategory.OPEN_CURLY);

                elifNode.Add(StmtList());

                Expect(TokenCategory.CLOSE_CURLY);
            }
            return elifList;
        }
        public Node Else()
        {
            if (Current == TokenCategory.ELSE)
            {
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.OPEN_CURLY);
                Node stmtList = StmtList();
                Expect(TokenCategory.CLOSE_CURLY);
                return stmtList;
            }
            return new StmtList();
        }
        public Node While()
        {
            Node whileNode = new While() { AnchorToken = Expect(TokenCategory.WHILE) };
            Expect(TokenCategory.OPEN_PAR);
            whileNode.Add(Expr());
            Expect(TokenCategory.CLOSE_PAR);
            Expect(TokenCategory.OPEN_CURLY);
            whileNode.Add(StmtList());
            Expect(TokenCategory.CLOSE_CURLY);
            return whileNode;
        }

        public Node Expr()
        {
            return ExprOr();
        }
        public Node ExprOr()
        {
            Node lhs = ExprAnd();
            while (Current == TokenCategory.OR)
            {
                Node orOp = new Or() { AnchorToken = Expect(TokenCategory.OR) };
                orOp.Add(lhs);
                orOp.Add(ExprAnd());
                lhs = orOp;
            }
            return lhs;
        }
        public Node ExprAnd()
        {
            Node lhs = ExprComp();
            while (Current == TokenCategory.AND)
            {
                Node andOp = new And() { AnchorToken = Expect(TokenCategory.AND) };
                andOp.Add(lhs);
                andOp.Add(ExprComp());
                lhs = andOp;
            }
            return lhs;
        }
        public Node ExprComp()
        {
            Node lhs = ExprRel();
            while (Current == TokenCategory.EQUALTO || Current == TokenCategory.NOTEQUALTO)
            {
                Node compOp;
                switch (Current)
                {
                    case TokenCategory.EQUALTO:
                        compOp = new Eq() { AnchorToken = Expect(TokenCategory.EQUALTO) };
                        break;
                    case TokenCategory.NOTEQUALTO:
                        compOp = new Neq() { AnchorToken = Expect(TokenCategory.NOTEQUALTO) };
                        break;
                    default:
                        throw new SyntaxError(new HashSet<TokenCategory>() { TokenCategory.EQUALTO, TokenCategory.NOTEQUALTO }, tokenStream.Current);
                }
                compOp.Add(lhs);
                compOp.Add(ExprRel());
                lhs = compOp;
            }
            return lhs;
        }
        public Node ExprRel()
        {
            Node lhs = ExprAdd();
            while (Current == TokenCategory.LESS || Current == TokenCategory.LESSEQUAL ||
              Current == TokenCategory.GREATER || Current == TokenCategory.GREATEREQUAL
            )
            {
                Node relOp;
                switch (Current)
                {
                    case TokenCategory.LESS:
                        relOp = new Lt() { AnchorToken = Expect(TokenCategory.LESS) };
                        break;
                    case TokenCategory.LESSEQUAL:
                        relOp = new Leq() { AnchorToken = Expect(TokenCategory.LESSEQUAL) };
                        break;
                    case TokenCategory.GREATER:
                        relOp = new Gt() { AnchorToken = Expect(TokenCategory.GREATER) };
                        break;
                    case TokenCategory.GREATEREQUAL:
                        relOp = new Gte() { AnchorToken = Expect(TokenCategory.GREATEREQUAL) };
                        break;
                    default:
                        throw new SyntaxError(new HashSet<TokenCategory>() { TokenCategory.LESS, TokenCategory.LESSEQUAL, TokenCategory.GREATER, TokenCategory.GREATEREQUAL }, tokenStream.Current);
                }
                relOp.Add(lhs);
                relOp.Add(ExprAdd());
                lhs = relOp;
            }
            return lhs;
        }
        public Node ExprAdd()
        {
            Node lhs = ExprMul();
            while (Current == TokenCategory.PLUS || Current == TokenCategory.NEG)
            {
                Node addOp;
                switch (Current)
                {
                    case TokenCategory.PLUS:
                        addOp = new Add() { AnchorToken = Expect(TokenCategory.PLUS) };
                        break;
                    case TokenCategory.NEG:
                        addOp = new Sub() { AnchorToken = Expect(TokenCategory.NEG) };
                        break;
                    default:
                        throw new SyntaxError(new HashSet<TokenCategory>() { TokenCategory.PLUS, TokenCategory.NEG }, tokenStream.Current);
                }
                addOp.Add(lhs);
                addOp.Add(ExprMul());
                lhs = addOp;
            }
            return lhs;
        }
        public Node ExprMul()
        {
            Node lhs = ExprUnary();
            while (Current == TokenCategory.TIMES || Current == TokenCategory.DIV || Current == TokenCategory.MOD)
            {
                Node mulOp;
                switch (Current)
                {
                    case TokenCategory.TIMES:
                        mulOp = new Mul() { AnchorToken = Expect(TokenCategory.TIMES) };
                        break;
                    case TokenCategory.DIV:
                        mulOp = new Div() { AnchorToken = Expect(TokenCategory.DIV) };
                        break;
                    case TokenCategory.MOD:
                        mulOp = new Mod() { AnchorToken = Expect(TokenCategory.MOD) };
                        break;
                    default:
                        throw new SyntaxError(new HashSet<TokenCategory>() { TokenCategory.TIMES, TokenCategory.DIV, TokenCategory.MOD }, tokenStream.Current);
                }
                mulOp.Add(lhs);
                mulOp.Add(ExprUnary()); // rhs
                lhs = mulOp;
            }
            return lhs;
        }
        public Node ExprUnary()
        {
            switch (Current)
            {
                case TokenCategory.PLUS:
                case TokenCategory.NEG:
                case TokenCategory.NOT:
                    var opUnary = OpUnary();
                    opUnary.Add(ExprUnary());
                    return opUnary;
                case TokenCategory.ID:
                case TokenCategory.LIT_INT:
                case TokenCategory.TRUE:
                case TokenCategory.FALSE:
                case TokenCategory.LIT_CHAR:
                case TokenCategory.LIT_STR:
                case TokenCategory.OPEN_BRAC:
                case TokenCategory.OPEN_PAR:
                    return ExprPrimary();
                default:
                    ISet<TokenCategory> set = firstOfExpression;
                    set.UnionWith(firstOfOpUnary);
                    throw new SyntaxError(set, tokenStream.Current);
            }
        }
        public Node OpUnary()
        {
            switch (Current)
            {
                case TokenCategory.PLUS:
                    return new Plus() { AnchorToken = Expect(TokenCategory.PLUS) };
                case TokenCategory.NEG:
                    return new Neg() { AnchorToken = Expect(TokenCategory.NEG) };
                case TokenCategory.NOT:
                    return new Not() { AnchorToken = Expect(TokenCategory.NOT) };
                default:
                    throw new SyntaxError(firstOfOpUnary, tokenStream.Current);
            }
        }
        public Node ExprPrimary()
        {
            switch (Current)
            {
                case TokenCategory.ID:
                    Token identifier = Expect(TokenCategory.ID);
                    if (Current == TokenCategory.OPEN_PAR)
                    {
                        return FunCall(identifier);
                    }
                    else
                    {
                        return new Identifier() { AnchorToken = identifier };
                    }
                case TokenCategory.LIT_INT:
                    return new IntLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.LIT_INT)
                    };
                case TokenCategory.TRUE:
                    return new IntLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.TRUE)
                    };
                case TokenCategory.FALSE:
                    return new IntLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.FALSE)
                    };
                case TokenCategory.LIT_CHAR:
                    return new IntLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.LIT_CHAR)
                    };
                case TokenCategory.LIT_STR:
                    return new StringLiteral()
                    {
                        AnchorToken = Expect(TokenCategory.LIT_STR)
                    };
                case TokenCategory.OPEN_BRAC:
                    return Array();
                case TokenCategory.OPEN_PAR:
                    Expect(TokenCategory.OPEN_PAR);
                    Node expr = Expr();
                    Expect(TokenCategory.CLOSE_PAR);
                    return expr;
                default:
                    throw new SyntaxError(firstOfExpression, tokenStream.Current);
            }
        }
        public Node Array()
        {
            Token openBrac = Expect(TokenCategory.OPEN_BRAC);
            Node exprList = ExprList();
            Expect(TokenCategory.CLOSE_BRAC);
            Node arr = new Array() { AnchorToken = openBrac };
            arr.Add(exprList);
            return arr;
        }

    }
}
