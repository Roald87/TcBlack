namespace TcBlackCore
{
    public abstract class CodeLineBase
    {
        internal string unformattedCode;

        protected CodeLineBase(string unformattedCode)
        {
            this.unformattedCode = unformattedCode;
        }

        public abstract string Format(ref int indents);
    }
}