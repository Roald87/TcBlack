namespace TcBlack
{
    public class VariableBlockStart : StatementBase
    {
        public VariableBlockStart(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            string formattedCode = 
                _singleIndent.Repeat(indents) + _unformattedCode.Trim();
            indents += 1;

            return formattedCode;
        }
    }
}