namespace Hydra_compiler
{
    class Program : Node { } // children: (VarDef, FunDef)[]
    class Identifier : Node { }
    class VarDef : Node { } // children: (Identifier, Assign)[]
    class Assign : Node { } // children: [Identifier, Expr]
    class FunDef : Node { } // children: [IdParamList, VarDefList, StmtList]
    class IdParamList : Node { } // children: (Identifier)[]
    class VarDefList : Node { } // children: (VarDef)[]
    class StmtList : Node { } // children: (StmtID, If, While, Break, Return)[]
    class StmtID : Node { } // children: (Assign, PlusPlus, LessLess, Increment, Decrement, FunCall)[]
    class FunCall : Node { } // children: [Identifier, ExprList]
    class If : Node { } // childern: [Expr, StmtLst, ElseIfList, StmtLst]
    class ElseIfList : Node { } // childern: (ElseIfNode)[]
    class ElseIfNode : Node { } // children: [Expr, StmtLst]
    class While : Node { } // childern: [Expr, StmtLst]
    class Return : Node { } // children [Expr]
    class Break : Node { }
    class Empty : Node { }

    class Expr : Node { }
    class ExprList : Node { } // children: (Expr)[]

    class OpBinary : Expr { } // children: [Expr, Expr]
    class Or : OpBinary { }
    class And : OpBinary { }
    class Eq : OpBinary { }
    class Neq : OpBinary { }
    class Lt : OpBinary { }
    class Gt : OpBinary { }
    class Leq : OpBinary { }
    class Gte : OpBinary { }

    class Add : OpBinary { }
    class Increment : OpBinary { }
    class Sub : OpBinary { }
    class Decrement : OpBinary { }

    class Mul : OpBinary { }
    class Div : OpBinary { }
    class Mod : OpBinary { }


    class OpUnary : Expr { } // children: [Expr]
    class PlusPlus : Expr { } // children: [Expr]
    class LessLess : Expr { } // children: [Expr]
    class Plus : OpUnary { }
    class Neg : OpUnary { }
    class Not : OpUnary { }

    // primary expr
    class IntLiteral : Expr { } // children: []
    class StringLiteral : Expr { } // children: []
    class Array : Expr { } // children: [ExprList]
}