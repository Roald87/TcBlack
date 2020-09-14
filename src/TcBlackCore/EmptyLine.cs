namespace TcBlackCore
{
    public class EmptyLine : CodeLineBase
    {
        public EmptyLine(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            return Global.indentation.Repeat(indents);
        }
    }
}
