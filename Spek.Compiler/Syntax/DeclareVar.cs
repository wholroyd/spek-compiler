namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// var <ident> = <expr>
    /// </summary>
    public class DeclareVar : Stmt
    {
        public string Ident;
        public Expr Expr;
    }
}