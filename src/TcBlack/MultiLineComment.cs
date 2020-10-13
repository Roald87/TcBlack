using System.Text.RegularExpressions;

namespace TcBlack
{
    class MultiLineComment : CodeLineBase
    {
        public MultiLineComment(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            int start = _unformattedCode.IndexOf('(') + 2;
            int end = _unformattedCode.LastIndexOf(')');
            return (
                $"{Global.indentation.Repeat(indents)}(*{Global.lineEnding}"
                + _unformattedCode.Substring(start, end - start - 3)
                + $"{Global.lineEnding}{Global.indentation.Repeat(indents)}*)"
            );
        }
    }
}
