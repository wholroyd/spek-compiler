namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// <stmt> ; <stmt>
    /// </summary>
    public class Sequence : Stmt
    {
        public Stmt First;
        public Stmt Second;
    }
}