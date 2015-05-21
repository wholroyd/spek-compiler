namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// <ident> := <char> <ident_rest>*
    /// <ident_rest> := <char> | <digit>
    /// </summary>
    public class Variable : Expr
    {
        public string Ident;
    }
}