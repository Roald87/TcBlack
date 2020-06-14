using Xunit;
using TcBlack;

namespace TcBlackTests
{
    public class EmptyLineTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("\t\t")]
        [InlineData("\t      ")]
        public void DifferentEmptyLines(string emptyLine)
        {
            string lineEnding = "\n";
            EmptyLine line = new EmptyLine(emptyLine, " ", lineEnding);

            uint indents = 0;
            Assert.Equal("", line.Format(ref indents));
        }
    }
}
