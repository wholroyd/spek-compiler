namespace Spek.Compiler.Console
{
    using System;
    using System.IO;

    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ssc.exe program.spek");
                return;
            }

            try
            {
                Scanner scanner;
                using (TextReader input = File.OpenText(args[0]))
                {
                    scanner = new Scanner(input);
                }

                var parser = new Parser(scanner.Tokens);
                var binary = new CodeGen(parser.Result, Path.GetFileNameWithoutExtension(args[0]) + ".exe");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}