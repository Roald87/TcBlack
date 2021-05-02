namespace TcBlackCore
{
    public class EmptyLine : CodeLineBase
    {
        public EmptyLine(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref int indents)
        {
            return Globals.indentation.Repeat(indents);
        }
    }
}
