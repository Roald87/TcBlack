using Xunit;
using TcBlackCore;

namespace TcBlackTests
{
    [Collection("Sequential")]
    public class VarBlockStartTests
    {
        [Theory]
        [InlineData("       VAR", 0, "VAR", 1)]
        [InlineData("   VAR_INPUT     ", 1, "    VAR_INPUT", 2)]
        public void DifferentIndents(
            string originalCode, 
            int indents, 
            string expectedCode, 
            int expectedIndents
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            VariableBlockStart var = new VariableBlockStart(originalCode);
            Assert.Equal(expectedCode, var.Format(ref indents));
            Assert.Equal(expectedIndents, indents);
        }
    }
}
