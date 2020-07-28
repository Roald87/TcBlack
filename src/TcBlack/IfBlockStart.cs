using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TcBlack
{
    public class IfBlockStart : CodeLineBase
    {
        protected static Regex StatementRegex = new Regex(
            @"\s+(and|or|and_then|or_else)\s+",
            RegexOptions.IgnoreCase
        );

        protected static Regex IfRegex = new Regex(
            @"\s*if\s+",
            RegexOptions.IgnoreCase
        );

        protected static Regex ThenRegex = new Regex(
            @"\s+then\s*",
            RegexOptions.IgnoreCase
        );

        public struct IfStatement
        {
            public IfStatement(
                Statement[] stms,
                string[] ops
            )
            {
                Stms = stms;
                Ops = ops;
            }

            public Statement[] Stms { get; }
            public string[] Ops { get; }
        }

        public IfBlockStart(string unformattedCode) : base(unformattedCode)
        {
        }

        public IfStatement Tokenize()
        {
            string[] lines = StatementRegex.Split(_unformattedCode);
            lines[0] = IfRegex.Replace(lines[0], "");
            lines[lines.Length - 1] = ThenRegex.Replace(lines[lines.Length - 1], "");

            List<Statement> stms = new List<Statement>();
            List<string> ops = new List<string>();
            if (lines.Length == 1)
            {
                stms.Add(new Statement(lines[0]));
            }
            else
            {
                for (int i = 0; i < lines.Length - 1; i += 2)
                {
                    stms.Add(new Statement(lines[i]));
                    ops.Add(lines[i + 1].ToUpper());
                }
                stms.Add(new Statement(lines.Last()));
            }

            return new IfStatement(stms.ToArray(), ops.ToArray());
        }

        protected string FormatSingle(ref uint indents, IfStatement stm)
        {
            uint dummyIndent = 0;
            if (stm.Stms.Length == 1)
            {
                return (
                    Global.indentation.Repeat(indents)
                    + $"IF {stm.Stms[0].Format(ref dummyIndent)} THEN"
                );
            }
            else
            {
                string formattedCode = $"{Global.indentation.Repeat(indents)}IF ";
                for (int i = 0; i < stm.Ops.Length; i++)
                {
                    formattedCode += (
                        stm.Stms[i].Format(ref dummyIndent)
                        + $" {stm.Ops[i]} "
                    );
                }
                formattedCode +=
                    $"{stm.Stms[stm.Stms.Length - 1].Format(ref dummyIndent)} THEN";
                return formattedCode;
            }
        }

        protected string FormatMultiple(ref uint indents, IfStatement stm)
        {
            if (stm.Stms.Length == 1)
            {
                return FormatSingle(ref indents, stm);
            }

            string formattedCode =
                $"{Global.indentation.Repeat(indents)}IF{Global.lineEnding}";
            indents += 1;

            formattedCode += $"{stm.Stms[0].Format(ref indents)}";

            for (int i = 1; i < stm.Stms.Length; i++)
            {
                formattedCode += (
                    $"{Global.indentation.Repeat(indents)}{stm.Ops[i - 1]} "
                    + stm.Stms[i].Format(ref indents)
                    + Global.lineEnding
                );
            }
            indents -= 1;
            formattedCode +=
                $"{Global.indentation.Repeat(indents)}THEN";

            return formattedCode;
        }

        public override string Format(ref uint indents)
        {
            IfStatement stm = Tokenize();
            string formattedCode = FormatSingle(ref indents, stm);
            if (formattedCode.Length > 88 && stm.Stms.Length > 1)
            {
                formattedCode = FormatMultiple(ref indents, stm);
            }
            indents += 1;
            return formattedCode;
        }
    }
}