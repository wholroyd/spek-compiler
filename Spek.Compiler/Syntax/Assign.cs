namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// <ident> = <expr>
    /// </summary>
    public class Assign : Stmt
    {
        public string Ident;
        public Expr Expr;
    }
}