namespace TcBlack
{
    public class UnknownCodeType : CodeLineBase
    {
        public UnknownCodeType(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            return _singleIndent.Repeat(indents) + _unformattedCode;
        }
    }
}
