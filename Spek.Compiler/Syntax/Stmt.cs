/* <stmt> := var <ident> = <expr>
	| <ident> = <expr>
	| for <ident> = <expr> to <expr> do <stmt> end
	| read_int <ident>
	| print <expr>
	| <stmt> ; <stmt>
  */

namespace Spek.Compiler.Syntax
{
    public abstract class Stmt
    {
    }
}