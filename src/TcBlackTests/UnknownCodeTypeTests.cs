using Xunit;
using TcBlack;

namespace TcBlackTests
{
    public class UnknownCodeTypeTests
    {
        [Theory]
        [InlineData("some gibberisch")]
        [InlineData("// Or some comment")]
        [InlineData("\t      (* with some spaces *)")]
        [InlineData("  {attribute 'hide'}")]
        public void DifferentEmptyLines(string unformattedCode)
        {
            Global.indentation = " ";
            Global.lineEnding = "\n";
            var line = new UnknownCodeType(unformattedCode);

            uint indents = 0;
            Assert.Equal(unformattedCode, line.Format(ref indents));
        }
    }
}
