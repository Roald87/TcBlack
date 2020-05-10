namespace TcBlack
{
    public abstract class StatementBase
    {
        protected string _unformattedCode;
        protected string _singleIndent;
        protected string _lineEnding;

        public StatementBase(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        )
        {
            _unformattedCode = unformattedCode;
            _singleIndent = singleIndent;
            _lineEnding = lineEnding;
        }

        public abstract string Format(ref uint indents);
    }
}