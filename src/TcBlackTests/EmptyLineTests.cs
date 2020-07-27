﻿using Xunit;
using TcBlack;

namespace TcBlackTests
{
    public class EmptyLineTests
    {
        [Theory]
        [InlineData("", 0, "")]
        [InlineData("\t\t", 1, " ")]
        [InlineData("\t      ", 2, "  ")]
        public void DifferentEmptyLines(
            string unformattedCode, uint initialIndents, string expected
        )
        {
            Global.indentation = " ";
            Global.lineEnding = "\n";
            EmptyLine line = new EmptyLine(unformattedCode);

            uint indents = initialIndents;
            Assert.Equal(expected, line.Format(ref indents));
        }
    }
}
