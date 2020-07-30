using System.Linq;
using System.Text.RegularExpressions;
using System;


namespace TcBlack
{
    class ImplementationCode : CompositeCode
    {
        public ImplementationCode(string unformattedCode) : base(unformattedCode)
        {
        }

        protected static Regex caseNumber = new Regex(
            @"(?:-\s*)?\d+(?:\s*\.\.\s*(?:-\s*)\d+)?\s*:\s*(\/\/[^\n]+|\(\*.*?\*\))?$"
        );
        protected static Regex caseVariable = new Regex(
            @"[\w\d\[\]\.\-\+\*\/\s]+\s*:\s*(\/\/[^\n]+|\(\*.*?\*\))?$"
        );

        public new CompositeCode Tokenize()
        {
            string[] lines = _unformattedCode.Split(
                new[] { Global.lineEnding }, StringSplitOptions.None
            );
            string line = "";
            bool findBlockEnd = false;
            bool findFunctionEnd = false;
            uint insideCaseBlock = 0;
            string blockEnd = "";
            for (int i = 0; i < lines.Length-1; i++)
            {
                if (findBlockEnd || findFunctionEnd)
                {
                    // Add current line to full line since looking for end condition
                    line += $" {lines[i].Trim()}";
                    if (findBlockEnd && line.EndsWith(blockEnd, StringComparison.OrdinalIgnoreCase))
                    {
                        findBlockEnd = false;
                    }
                    if (findFunctionEnd && line.Count(f => f == '(') - line.Count(f => f == ')') == 0)
                    {
                        findFunctionEnd = false;
                    }
                    if (findBlockEnd || findFunctionEnd)
                    {
                        // If still looking for end condition continue
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
                    Add(new EmptyLine(line));
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
                        Add(new IfBlockStart(line));
                    }
                }
                else if (line.Equals("else", StringComparison.OrdinalIgnoreCase))
                {
                    Add(new IfBlockElse(line));
                }
                else if (line.StartsWith("case", StringComparison.OrdinalIgnoreCase))
                {
                    insideCaseBlock += 1;
                    Add(new CaseBlockStart(line));
                }
                else if (insideCaseBlock > 0 && (caseNumber.IsMatch(line) || caseVariable.IsMatch(line)))
                {
                    Add(new CaseNumberBlock(line));
                }
                else if (line.Contains('('))
                {
                    // Function call
                    if (line.Count(f => f == '(') - line.Count(f => f == ')') > 0)
                    {
                        // Multiline function call
                        findFunctionEnd = true;
                        continue;
                    }
                    else
                    {
                        Add(new FunctionStatement(line));
                    }
                }
                else if (line.StartsWith("end_case", StringComparison.OrdinalIgnoreCase))
                {
                    insideCaseBlock -= 1;
                    Add(new CaseBlockEnd(line));
                }
                else if (line.StartsWith("end_", StringComparison.OrdinalIgnoreCase))
                {
                    Add(new VariableBlockEnd(line));
                }
                else if (LooksLikeVariableAssignment(line))
                {
                    Add(new VariableAssignment(line));
                }
                else
                {
                    Add(new UnknownCodeType(line));
                }
            }

            RemoveAllEmptyLinesAtTheEnd();

            return this;
        }

        private bool LooksLikeVariableAssignment(string codeLine)
        {
            var code = new Statement(codeLine).Tokenize();
            return code.LeftOperand != "" && code.RightOperand != "";
        }
    }
}
