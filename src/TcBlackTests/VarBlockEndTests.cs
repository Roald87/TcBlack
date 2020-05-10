using Xunit;
using TcBlack;

namespace TcBlackTests
{
    public class VarBlockEndTests
    {
        [Theory]
        [InlineData("      END_VAR", 1, "END_VAR", 0)]
        [InlineData("   END_VAR     ", 2, "    END_VAR", 1)]
        [InlineData("   END_VAR     ", 0, "END_VAR", 0)]
        public void DifferentIndents(
            string originalCode, 
            uint indents, 
            string expectedCode, 
            uint expectedIndents
        )
        {
            VariableBlockEnd var = 
                new VariableBlockEnd(originalCode, "    ", "\n");
            Assert.Equal(expectedCode, var.Format(ref indents));
            Assert.Equal(expectedIndents, indents);
        }
    }
}
