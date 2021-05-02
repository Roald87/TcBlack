using Xunit;
using TcBlackCore;

namespace TcBlackTests
{
    [Collection("Sequential")]
    public class UnknownCodeTypeTests
    {
        [Theory]
        [InlineData("some gibberisch")]
        [InlineData("// Or some comment")]
        [InlineData("\t      (* with some spaces *)")]
        [InlineData("  {attribute 'hide'}")]
        public void DifferentEmptyLines(string unformattedCode)
        {
            Globals.indentation = " ";
            Globals.lineEnding = "\n";
            var line = new UnknownCodeType(unformattedCode);

            int indents = 0;
            Assert.Equal(unformattedCode, line.Format(ref indents));
        }
    }
}
