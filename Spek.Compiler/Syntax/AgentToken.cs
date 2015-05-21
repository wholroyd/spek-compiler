namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// <modifier> agent <ident>
    /// </summary>
    public class AgentToken : Stmt
    {
        public string Ident;

        public string Modifier;

        public Stmt body;
    }
}