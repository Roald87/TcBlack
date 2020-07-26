namespace TcBlack
{
    public class VariableBlockEnd : CodeLineBase
    {
        public VariableBlockEnd(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            indents = (indents == 0) ? 0 : indents -= 1;

            string formattedCode =
                Global.indentation.Repeat(indents) + _unformattedCode.Trim();

            return formattedCode;
        }
    }
}