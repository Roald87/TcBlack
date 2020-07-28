using System.Text.RegularExpressions;

namespace TcBlack
{
    public abstract class CodeLineBase
    {
        protected string _unformattedCode;
        protected string comment;

        protected static Regex commentRegex = new Regex(
            @"\s*(\/\/[^\n]+|\(\*.*?\*\))?"
        );
        protected static Regex commentFormatRegex = new Regex(
            @"\s*\/\/\s*"
        );

        protected bool RemoveComment()
        {
            Match match = commentRegex.Match(_unformattedCode);
            if (match.Length > 0)
            {
                _unformattedCode = _unformattedCode.Replace(match.Groups[0].Value, "");
                comment = match.Groups[1].Value;
                return true;
            }
            return false;
        }
        protected string FormatComment()
        {
            if (comment == null)
            {
                return "";
            }
            return commentFormatRegex.Replace(comment, " // ");
        }

        public CodeLineBase(string unformattedCode)
        {
            _unformattedCode = unformattedCode;
        }

        public abstract string Format(ref uint indents);
    }
}