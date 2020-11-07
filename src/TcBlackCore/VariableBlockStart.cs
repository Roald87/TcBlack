namespace TcBlackCore
{
    public class VariableBlockStart : CodeLineBase
    {
        public VariableBlockStart(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref int indents)
        {
            string formattedCode = 
                Global.indentation.Repeat(indents) + unformattedCode.Trim();
            indents += 1;

            return formattedCode;
        }
    }
}