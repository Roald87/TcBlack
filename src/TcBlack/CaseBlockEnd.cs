namespace TcBlack
{
    public class CaseBlockEnd : CodeLineBase
    {
        public CaseBlockEnd(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref uint indents)
        {
            if (indents >= 2)
            {
                indents -= 2;
            }

            string formattedCode =
                Global.indentation.Repeat(indents) + _unformattedCode.Trim();

            return formattedCode;
        }
    }
}
