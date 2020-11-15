namespace TcBlackCore
{
    public class VariableBlockEnd : CodeLineBase
    {
        public VariableBlockEnd(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref int indents)
        {
            indents = (indents == 0) ? 0 : indents -= 1;

            string formattedCode =
                Globals.indentation.Repeat(indents) + unformattedCode.Trim();

            return formattedCode;
        }
    }
}