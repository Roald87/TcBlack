using System.Text.RegularExpressions;

namespace TcBlack
{
    class CaseBlockStart : CodeLineBase
    {
        protected static Regex CaseRegex = new Regex(
            @"\s*case\s+([\w\d\[\]\-\+\/\.\*]+)\s+of\s*",
            RegexOptions.IgnoreCase
        );

        public CaseBlockStart(string unformattedCode) : base(unformattedCode)
        {
        }

        public string Tokenize()
        {
            Match match = CaseRegex.Match(_unformattedCode);
            if (match.Length > 0)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        public override string Format(ref uint indents)
        {
            string formated =
                $"{Global.indentation.Repeat(indents)}CASE {Tokenize()} OF";
            indents += 2;
            return formated;
        }
    }
}
