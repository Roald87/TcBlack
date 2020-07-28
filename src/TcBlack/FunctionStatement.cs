using System.Text.RegularExpressions;

namespace TcBlack
{
    class FunctionStatement : CodeLineBase
    {
        public FunctionStatement(string unformattedCode) : base(unformattedCode)
        {
        }

        public string Tokenize()
        {
            return _unformattedCode;
        }

        public override string Format(ref uint indents)
        {
            string formated =
                $"{Global.indentation.Repeat(indents)}{Tokenize()}";
            return formated;
        }
    }
}
