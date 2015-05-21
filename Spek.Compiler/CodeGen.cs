namespace Spek.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;

    using Spek.Compiler.Syntax;

    public sealed class CodeGen
    {
        private readonly ILGenerator il;

        private readonly Dictionary<string, LocalBuilder> symbolTable;

        public CodeGen(Stmt stmt, string moduleName)
        {
            if (Path.GetFileName(moduleName) != moduleName)
            {
                throw new Exception("can only output into current directory!");
            }

            var name = new AssemblyName(Path.GetFileNameWithoutExtension(moduleName));
            var asmb = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            var modb = asmb.DefineDynamicModule(moduleName);
            var typeBuilder = modb.DefineType("Foo");

            var methb = typeBuilder.DefineMethod("Main", MethodAttributes.Static, typeof(void), Type.EmptyTypes);

            // CodeGenerator
            this.il = methb.GetILGenerator();
            this.symbolTable = new Dictionary<string, LocalBuilder>();

            // Go Compile!
            this.GenStmt(stmt);

            this.il.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            modb.CreateGlobalFunctions();
            asmb.SetEntryPoint(methb);
            asmb.Save(moduleName);
            this.symbolTable = null;
            this.il = null;
        }

        private void GenStmt(Stmt stmt)
        {
            if (stmt is Sequence)
            {
                var seq = (Sequence)stmt;
                this.GenStmt(seq.First);
                this.GenStmt(seq.Second);
            }		
        
            else if (stmt is DeclareVar)
            {
                // declare a local
                var declare = (DeclareVar)stmt;
                this.symbolTable[declare.Ident] = this.il.DeclareLocal(this.TypeOfExpr(declare.Expr));

                // set the initial value
                var assign = new Assign();
                assign.Ident = declare.Ident;
                assign.Expr = declare.Expr;
                this.GenStmt(assign);
            }        
        
            else if (stmt is Assign)
            {
                var assign = (Assign)stmt;
                this.GenExpr(assign.Expr, this.TypeOfExpr(assign.Expr));
                this.Store(assign.Ident, this.TypeOfExpr(assign.Expr));
            }			
            else if (stmt is Print)
            {
                // the "print" statement is an alias for System.Console.WriteLine. 
                // it uses the string case
                this.GenExpr(((Print)stmt).Expr, typeof(string));
                this.il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
            }	

            else if (stmt is ReadInt)
            {
                this.il.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null));
                this.il.Emit(OpCodes.Call, typeof(int).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null));
                this.Store(((ReadInt)stmt).Ident, typeof(int));
            }
            else if (stmt is ForLoop)
            {
                // example: 
                // for x = 0 to 100 do
                //   print "hello";
                // end;

                // x = 0
                var forLoop = (ForLoop)stmt;
                var assign = new Assign();
                assign.Ident = forLoop.Ident;
                assign.Expr = forLoop.From;
                this.GenStmt(assign);			
                // jump to the test
                var test = this.il.DefineLabel();
                this.il.Emit(OpCodes.Br, test);

                // statements in the body of the for loop
                var body = this.il.DefineLabel();
                this.il.MarkLabel(body);
                this.GenStmt(forLoop.Body);

                // to (increment the value of x)
                this.il.Emit(OpCodes.Ldloc, this.symbolTable[forLoop.Ident]);
                this.il.Emit(OpCodes.Ldc_I4, 1);
                this.il.Emit(OpCodes.Add);
                this.Store(forLoop.Ident, typeof(int));

                // **test** does x equal 100? (do the test)
                this.il.MarkLabel(test);
                this.il.Emit(OpCodes.Ldloc, this.symbolTable[forLoop.Ident]);
                this.GenExpr(forLoop.To, typeof(int));
                this.il.Emit(OpCodes.Blt, body);
            }
            else
            {
                throw new Exception("don't know how to gen a " + stmt.GetType().Name);
            }
        }    
    	
        private void Store(string name, Type type)
        {
            if (this.symbolTable.ContainsKey(name))
            {
                var locb = this.symbolTable[name];

                if (locb.LocalType == type)
                {
                    this.il.Emit(OpCodes.Stloc, this.symbolTable[name]);
                }
                else
                {
                    throw new Exception("'" + name + "' is of type " + locb.LocalType.Name + " but attempted to store value of type " + type.Name);
                }
            }
            else
            {
                throw new Exception("undeclared variable '" + name + "'");
            }
        }

        private void GenExpr(Expr expr, Type expectedType)
        {
            Type deliveredType;
		
            if (expr is StringLiteral)
            {
                deliveredType = typeof(string);
                this.il.Emit(OpCodes.Ldstr, ((StringLiteral)expr).Value);
            }
            else if (expr is IntLiteral)
            {
                deliveredType = typeof(int);
                this.il.Emit(OpCodes.Ldc_I4, ((IntLiteral)expr).Value);
            }        
            else if (expr is Variable)
            {
                var ident = ((Variable)expr).Ident;
                deliveredType = this.TypeOfExpr(expr);

                if (!this.symbolTable.ContainsKey(ident))
                {
                    throw new Exception("undeclared variable '" + ident + "'");
                }

                this.il.Emit(OpCodes.Ldloc, this.symbolTable[ident]);
            }
            else
            {
                throw new Exception("don't know how to generate " + expr.GetType().Name);
            }

            if (deliveredType != expectedType)
            {
                if (deliveredType == typeof(int) &&
                    expectedType == typeof(string))
                {
                    this.il.Emit(OpCodes.Box, typeof(int));
                    this.il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("ToString"));
                }
                else
                {
                    throw new Exception("can't coerce a " + deliveredType.Name + " to a " + expectedType.Name);
                }
            }

        }

        private Type TypeOfExpr(Expr expr)
        {
            var stringLiteral = expr as StringLiteral;
            if (stringLiteral != null)
            {
                return typeof(string);
            }
            var literal = expr as IntLiteral;
            if (literal != null)
            {
                return typeof(int);
            }

            var variable = expr as Variable;
            if (variable != null)
            {
                var var = variable;
                if (this.symbolTable.ContainsKey(var.Ident))
                {
                    var locb = this.symbolTable[var.Ident];
                    return locb.LocalType;
                }

                throw new Exception("undeclared variable '" + var.Ident + "'");
            }

            throw new Exception("don't know how to calculate the type of " + expr.GetType().Name);
        }
    }
}