namespace TcBlackCore
{
    public class VariableBlockStart : CodeLineBase
    {
        public VariableBlockStart(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            string formattedCode = 
                Global.indentation.Repeat(indents) + _unformattedCode.Trim();
            indents += 1;

            return formattedCode;
        }
    }
}