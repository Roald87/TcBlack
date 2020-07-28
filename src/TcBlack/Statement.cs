using System.Text.RegularExpressions;

namespace TcBlack
{
    public class Statement : CodeLineBase
    {
        protected static string operand = @"([\w\d\[\]\.()]+)";
        protected static Regex OperandRegex = new Regex(
           @"\s*"
           + operand
           + @"\s*(\+|\-|\*|\/|<=|>=|=<|=>|<>|<|>|:=|=)?\s*"
           + $"{operand}?"
           + @"\s*(;?)[\s;]*"
        );

        public Statement(string unformattedCode) : base(unformattedCode)
        {
        }

        public struct TcStatement
        {
            public TcStatement(
                string leftOperand,
                string operand,
                string rightOperand,
                string terminator
            )
            {
                LeftOperand = leftOperand;
                Operand = operand;
                RightOperand = rightOperand;
                Terminator = terminator;
            }

            public string LeftOperand { get; }
            public string Operand { get; }
            public string RightOperand { get; }
            public string Terminator { get; }
        }

        public TcStatement Tokenize()
        {
            Match match = OperandRegex.Match(_unformattedCode);
            if (match.Length > 0)
            {
                return new TcStatement(
                    match.Groups[1].Value,
                    match.Groups[2].Value,
                    match.Groups[3].Value,
                    match.Groups[4].Value
                );
            }
            else
            {
                return new TcStatement("", "", "", "");
            }
        }

        public override string Format(ref uint indents)
        {
            TcStatement stm = Tokenize();
            return (
                Global.indentation.Repeat(indents)
                + $"{stm.LeftOperand}"
                + (stm.Operand.Length > 0 ? $" {stm.Operand}" : "")
                + (stm.RightOperand.Length > 0 ? $" {stm.RightOperand}" : "")
                + stm.Terminator
            );
        }
    }
}
