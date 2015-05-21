namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// for <ident> = <expr> to <expr> do <stmt> end
    /// </summary>
    public class ForLoop : Stmt
    {
        public string Ident;
        public Expr From;
        public Expr To;
        public Stmt Body;
    }
}