using System.Linq;
using System.Text.RegularExpressions;

namespace TcBlack
{
    public class VariableAssignment : CodeLineBase
    {

        public struct TcAssignment
        {
            public TcAssignment(
                string leftOperand,
                string rightOperand
            )
            {
                LeftOperand = leftOperand;
                RightOperand = rightOperand;
            }

            public string LeftOperand { get; }
            public string RightOperand { get; }
        }

        public VariableAssignment(
            string unformattedCode
        ) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            TcAssignment assign = Tokenize();
            string formated = (
                Global.indentation.Repeat(indents)
                + $"{ assign.LeftOperand} := {assign.RightOperand}"
            );
            if (formated.Last() != ';')
            {
                formated += ';';
            }
            return formated;
        }

        public TcAssignment Tokenize()
        {
            Match match = Regex.Match(_unformattedCode, @"^\s*([\w[\].]+)\s*:=\s*(.+)\s*;?\s*");
            if (match.Length > 0)
            {
                return new TcAssignment(
                    match.Groups[1].Value,
                    match.Groups[2].Value
                );
            }
            else
            {
                return new TcAssignment("", "");
            }
        }
    }
}
