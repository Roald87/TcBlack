using Xunit;
using TcBlack;

namespace TcBlackTests
{
    public class VarBlockStartTests
    {
        [Theory]
        [InlineData("       VAR", 0, "VAR", 1)]
        [InlineData("   VAR_INPUT     ", 1, "    VAR_INPUT", 2)]
        public void DifferentIndents(
            string originalCode, 
            uint indents, 
            string expectedCode, 
            uint expectedIndents
        )
        {
            Global.indentation = "    ";
            Global.lineEnding = "\n";
            VariableBlockStart var = new VariableBlockStart(originalCode);
            Assert.Equal(expectedCode, var.Format(ref indents));
            Assert.Equal(expectedIndents, indents);
        }
    }
}
