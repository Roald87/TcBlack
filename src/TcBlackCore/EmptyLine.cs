namespace TcBlackCore
{
    public class EmptyLine : CodeLineBase
    {
        public EmptyLine(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref int indents)
        {
            var notUsed = "test for static code analyses";
            return Globals.indentation.Repeat(indents);
        }
    }
}
