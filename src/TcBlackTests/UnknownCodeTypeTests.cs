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
            string lineEnding = "\n";
            var line = new UnknownCodeType(unformattedCode, " ", lineEnding);

            uint indents = 0;
            Assert.Equal(unformattedCode, line.Format(ref indents));
        }
    }
}
