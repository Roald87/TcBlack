using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace TcBlack
{
    class FunctionStatement : CodeLineBase
    {
        protected static Regex WhiteSpaceRegex = new Regex(@"\s+");
        public struct TcVariablePassing
        {
            public TcVariablePassing(
                string name,
                string terminate,
                string[] simple,
                string[] assignments,
                string[] returns
            )
            {
                Name = name;
                Terminate = terminate;
                Simple = simple;
                Assignments = assignments;
                Returns = returns;
            }

            public string Name { get; }
            public string Terminate { get; }
            public string[] Simple { get; }
            public string[] Assignments { get; }
            public string[] Returns { get; }
        }

        public FunctionStatement(string unformattedCode) : base(unformattedCode)
        {
        }

        public TcVariablePassing Tokenize()
        {
            int open = _unformattedCode.IndexOf('(');
            string name = _unformattedCode.Substring(0, open).Trim();
            int last = _unformattedCode.LastIndexOf(')');
            string terminate = _unformattedCode.Substring(last+1).Trim();
            string[] vars = _unformattedCode.Remove(last).Remove(0, open+1).Split(',');
            List<string> simple = new List<string>();
            List<string> assignments = new List<string>();
            List<string> returns = new List<string>();
            foreach(string var in vars)
            {
                if (var.Contains(":="))
                {
                    assignments.Add(WhiteSpaceRegex.Replace(var, ""));
                }
                else if (var.Contains("=>"))
                {
                    returns.Add(WhiteSpaceRegex.Replace(var, ""));
                }
                else
                {
                    simple.Add(WhiteSpaceRegex.Replace(var, ""));
                }
            }
            return new TcVariablePassing(
                name,
                terminate,
                simple.ToArray(),
                assignments.ToArray(),
                returns.ToArray()
            );
        }

        protected string FormatSingle(ref uint indents, TcVariablePassing func, bool multi)
        {
            string formated = $"{Global.indentation.Repeat(indents)}{func.Name}(";
            if (multi)
            {
                formated += Global.lineEnding;
            }
            foreach(
                string ea
                in func.Simple.Concat(func.Assignments).Concat(func.Returns)
            )
            {
                if (ea.Trim().Length == 0)
                {
                    continue;
                }
                if (multi)
                {
                    formated += (
                        $"{Global.indentation.Repeat(indents + 1)}"
                        + $"{ea},{Global.lineEnding}"
                    );
                }
                else
                {
                    formated += $"{ea}, ";
                }
            }
            if (multi)
            {
                return (
                    formated
                    + Global.indentation.Repeat(indents)
                    + $"){func.Terminate}"
                );
            }
            else
            {
                return $"{formated.Remove(formated.Length - 2)}){func.Terminate}";
            }
        }

        public override string Format(ref uint indents)
        {
            TcVariablePassing func = Tokenize();
            string formated = FormatSingle(ref indents, func, false);
            if (formated.Length > 88)
            {
                formated = FormatSingle(ref indents, func, true);
            }
            return formated;
        }
    }
}
