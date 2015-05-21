namespace Spek.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public sealed class Scanner
    {
        private readonly IList<object> result;

        public Scanner(TextReader input)
        {
            this.result = new List<object>();
            this.Scan(input);
        }

        public IList<object> Tokens
        {
            get { return this.result; }
        }

        #region ArithmiticConstants

        // Constants to represent arithmitic tokens. This could
        // be alternatively written as an enum.
        public static readonly object Add = new object();
        public static readonly object Sub = new object();
        public static readonly object Mul = new object();
        public static readonly object Div = new object();
        public static readonly object Semi = new object();
        public static readonly object Equal = new object();

        #endregion

        private void Scan(TextReader input)
        {
            while (input.Peek() != -1)
            {
                var ch = (char)input.Peek();

                // Scan individual tokens
                if (char.IsWhiteSpace(ch))
                {
                    // eat the current char and skip ahead!
                    input.Read();
                }
                else if (char.IsLetter(ch) || ch == '_')
                {
                    // keyword or identifier
                    this.ScanKeywordOrIdentifier(input, ch);
                }
                else if (ch == '"')
                {
                    // string literal
                    this.ScanStringLiteral(input);
                }
                else if (char.IsDigit(ch))
                {
                    // numeric literal

                    this.ScanNumericLiteral(input, ch);
                }
                else
                {
                    this.ScanOperatorOrDelimiter(input, ch);
                }
            }
        }

        private void ScanOperatorOrDelimiter(TextReader input, char ch)
        {
            switch (ch)
            {
                case '+':
                    input.Read();
                    this.result.Add(Add);
                    break;
                case '-':
                    input.Read();
                    this.result.Add(Sub);
                    break;
                case '*':
                    input.Read();
                    this.result.Add(Mul);
                    break;
                case '/':
                    input.Read();
                    this.result.Add(Div);
                    break;
                case '=':
                    input.Read();
                    this.result.Add(Equal);
                    break;
                case ';':
                    input.Read();
                    this.result.Add(Semi);
                    break;
                default:
                    throw new Exception("Scanner encountered unrecognized character '" + ch + "'");
            }
        }

        private void ScanNumericLiteral(TextReader input, char ch)
        {
            var accum = new StringBuilder();

            while (char.IsDigit(ch))
            {
                accum.Append(ch);
                input.Read();

                if (input.Peek() == -1)
                {
                    break;
                }

                ch = (char)input.Peek();
            }

            this.result.Add(int.Parse(accum.ToString()));
        }

        private void ScanStringLiteral(TextReader input)
        {
            char ch;
            var accum = new StringBuilder();

            input.Read(); // skip the '"'

            if (input.Peek() == -1)
            {
                throw new Exception("unterminated string literal");
            }

            while ((ch = (char)input.Peek()) != '"')
            {
                accum.Append(ch);
                input.Read();

                if (input.Peek() == -1)
                {
                    throw new Exception("unterminated string literal");
                }
            }

            // skip the terminating "
            input.Read();
            this.result.Add(accum);
        }

        private void ScanKeywordOrIdentifier(TextReader input, char ch)
        {
            var accum = new StringBuilder();

            while (char.IsLetter(ch) || ch == '_')
            {
                accum.Append(ch);
                input.Read();

                if (input.Peek() == -1)
                {
                    break;
                }

                ch = (char)input.Peek();
            }

            this.result.Add(accum.ToString());
        }
    }
}
