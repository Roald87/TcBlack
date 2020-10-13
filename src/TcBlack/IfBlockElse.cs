using System.Text.RegularExpressions;

namespace TcBlack
{
    class IfBlockElse : CodeLineBase
    {
        public IfBlockElse(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            RemoveComment();
            return $"{Global.indentation.Repeat(indents - 1)}ELSE{FormatComment()}";
        }
    }
}