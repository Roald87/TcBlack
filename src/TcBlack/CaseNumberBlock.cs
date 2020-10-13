using System.Text.RegularExpressions;

namespace TcBlack
{
    class CaseNumberBlock : CodeLineBase
    {
        public CaseNumberBlock(string unformattedCode) : base(unformattedCode)
        {
        }
        public string Tokenize()
        {
            return Regex.Replace(_unformattedCode, @"\s+", "");
        }

        public override string Format(ref uint indents)
        {
            RemoveComment();
            uint indent = 0;
            if (indents > 0)
            {
                // Prevent crashing, but this is parsing error somewhere
                indent = indents - 1;
            }
            string formated =
                $"{Global.indentation.Repeat(indent)}{Tokenize()}{FormatComment()}";
            return formated;
        }
    }
}
