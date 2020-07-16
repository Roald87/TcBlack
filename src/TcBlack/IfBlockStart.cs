using System.Text.RegularExpressions;

namespace TcBlack
{
    public class IfBlockStart : CodeLineBase
    {
        public IfBlockStart(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            string formattedCode =
                    _singleIndent.Repeat(indents) + _unformattedCode.Trim();
            if (formattedCode.Length > 88)
            {
                string[] lines = Regex.Split(
                    _unformattedCode,
                    @"(and|or|and_then|or_else)",
                    RegexOptions.IgnoreCase
                );
                formattedCode = _singleIndent.Repeat(indents) + lines[0];
                for (int i = 1; i < lines.Length-1; i+=2)
                {
                    formattedCode += (
                        _lineEnding
                        + _singleIndent.Repeat(indents + 1)
                        + lines[i].ToUpper()
                        + lines[i + 1].TrimEnd()
                    );
                }
                formattedCode = Regex.Replace(
                    formattedCode,
                    @"\s+then",
                    _lineEnding + _singleIndent.Repeat(indents) + "THEN",
                    RegexOptions.IgnoreCase
                );
            }
            indents += 1;

            return formattedCode;
        }
    }
}