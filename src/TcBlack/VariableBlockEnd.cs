namespace TcBlack
{
    public class VariableBlockEnd : StatementBase
    {
        public VariableBlockEnd(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            indents = (indents == 0) ? 0 : indents -= 1;

            string formattedCode =
                _singleIndent.Repeat(indents) + _unformattedCode.Trim();

            return formattedCode;
        }
    }
}