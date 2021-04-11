using System;

[assembly: CLSCompliant(true)]
namespace TcBlackCore
{
    public abstract class CodeLineBase
    {
        internal string unformattedCode;

        protected CodeLineBase(string unformattedCode)
        {
            this.unformattedCode = unformattedCode;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1045:DoNotPassTypesByReference", 
            MessageId = "0#",
            Justification = "Don't know an alternative."
        )]
        public abstract string Format(ref int indents);
    }
}