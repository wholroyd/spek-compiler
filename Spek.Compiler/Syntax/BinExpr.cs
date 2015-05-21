namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// <bin_expr> := <expr> <bin_op> <expr>
    /// </summary>
    public class BinExpr : Expr
    {
        public Expr Left;
        public Expr Right;
        public BinOp Op;
    }
}