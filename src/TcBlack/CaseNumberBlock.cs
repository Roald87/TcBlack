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
            string formated =
                $"{Global.indentation.Repeat(indents-1)}{Tokenize()}{FormatComment()}";
            return formated;
        }
    }
}
