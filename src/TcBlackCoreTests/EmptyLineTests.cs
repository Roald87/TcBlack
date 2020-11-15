using Xunit;
using TcBlackCore;

namespace TcBlackTests
{
    [Collection("Sequential")]
    public class EmptyLineTests
    {
        [Theory]
        [InlineData("", 0, "")]
        [InlineData("\t\t", 1, " ")]
        [InlineData("\t      ", 2, "  ")]
        public void DifferentEmptyLines(
            string unformattedCode, int initialIndents, string expected
        )
        {
            Globals.indentation = " ";
            Globals.lineEnding = "\n";
            EmptyLine line = new EmptyLine(unformattedCode);

            int indents = initialIndents;
            Assert.Equal(expected, line.Format(ref indents));
        }
    }
}
