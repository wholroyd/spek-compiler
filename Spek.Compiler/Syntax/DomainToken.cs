namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// domain <ident>
    /// </summary>
    public class DomainToken : Stmt
    {
        public string Ident;

        public Stmt body;
    }
}