namespace TcBlackCore
{
    public abstract class CodeLineBase
    {
        protected string _unformattedCode;

        protected CodeLineBase(string unformattedCode)
        {
            _unformattedCode = unformattedCode;
        }

        public abstract string Format(ref uint indents);
    }
}