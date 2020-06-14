namespace TcBlack
{
    public class UnknownCodeType : CodeLineBase
    {
        public UnknownCodeType(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            return _unformattedCode;
        }
    }
}
