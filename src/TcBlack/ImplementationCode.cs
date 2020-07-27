using System.Linq;
using System.Text.RegularExpressions;
using System;


namespace TcBlack
{
    class ImplementationCode : CompositeCode
    {
        public ImplementationCode(
            string unformattedCode
        ) : base(unformattedCode)
        {
        }

        public new CompositeCode Tokenize()
        {
            string[] lines = _unformattedCode.Split(
                new[] { Global.lineEnding }, StringSplitOptions.None
            );
            string line = "";
            bool findBlockEnd = false;
            string blockEnd = "";
            for (int i = 0; i < lines.Length-1; i++)
            {
                if (findBlockEnd)
                {
                    line += " " + lines[i].Trim();
                    if (line.EndsWith(blockEnd, StringComparison.OrdinalIgnoreCase))
                    {
                        findBlockEnd = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    line = lines[i].Trim();
                }
                if (line.Length == 0)
                {
                    if (codeLines.Count > 0 && codeLines.Last() is EmptyLine)
                    {
                        continue;
                    }
                    Add(new EmptyLine(unformattedCode: line));
                }
                else if (line.StartsWith("if", StringComparison.OrdinalIgnoreCase))
                {
                    if (!line.EndsWith("then", StringComparison.OrdinalIgnoreCase))
                    {
                        findBlockEnd = true;
                        blockEnd = "then";
                        continue;
                    }
                    else
                    {
                        Add(new IfBlockStart(unformattedCode: line));
                    }
                }
                else if (line.StartsWith("end_", StringComparison.OrdinalIgnoreCase))
                {
                    Add(new VariableBlockEnd(unformattedCode: line));
                }
                else if (LooksLikeVariableAssignment(line))
                {
                    Add(new VariableAssignment(unformattedCode: line));
                }
                else
                {
                    Add(new UnknownCodeType(unformattedCode: line));
                }
            }

            RemoveAllEmptyLinesAtTheEnd();

            return this;
        }

        public override string Format(ref uint indents)
        {
            return FixWhiteSpace(base.Format(ref indents));
        }

        private bool LooksLikeVariableAssignment(string codeLine)
        {
            var code = new VariableAssignment(codeLine).Tokenize();

            return code.LeftOperand != "" && code.RightOperand != "";
        }

        private static string AddSpaceBeforeAfter(Match m)
        {
            return " " + m.ToString().Trim() + " ";
        }

        private static string AddSpaceAfter(Match m)
        {
            return m.ToString().Trim() + " ";
        }

        private static string RemoveSpace(Match m)
        {
            return m.ToString().Trim();
        }

        protected static Regex RemoveSpaceRegex = new Regex(
            @"(\s*(?:\(|\))\s*)"
        );

        protected static Regex AddSpaceAfterRegex = new Regex(
            @"(\s*(?:,)\s*)"
        );

        protected static Regex AddSpaceBeforeAfterRegex = new Regex(
            @"(\s*(?:\+|\-|\*|\/|<=|>=|=<|=>|<|>|:=)\s*)"
        );

        protected string FixWhiteSpace(string code)
        {
            code = RemoveSpaceRegex.Replace(code, new MatchEvaluator(RemoveSpace));
            code = AddSpaceAfterRegex.Replace(code, new MatchEvaluator(AddSpaceAfter));
            code = AddSpaceBeforeAfterRegex.Replace(code, new MatchEvaluator(AddSpaceBeforeAfter));
            return code;
        }
    }
}
