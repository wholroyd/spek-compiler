namespace Spek.Compiler.Syntax
{
    /// <summary>
    /// <string> := " <string_elem>* "
    /// </summary>
    public class StringLiteral : Expr
    {
        public string Value;
    }
}