using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class ObjectDefinitionTests
    {
        [Theory]
        [InlineData("      FUNCTION Sum : BOOL", 1, "    FUNCTION Sum : BOOL", 1)]
        [InlineData("FUNCTION Sum_2     :  BOOL ", 1, "    FUNCTION Sum_2 : BOOL", 1)]
        [InlineData("   METHOD Adder :LREAL", 2, "        METHOD Adder : LREAL", 2)]
        [InlineData(
            "   FUNCTION_BLOCK Subtract: REAL", 0, "FUNCTION_BLOCK Subtract : REAL", 0
        )]
        public void DifferentIndents(
            string originalCode,
            uint indents,
            string expectedCode,
            uint expectedIndents
        )
        {
            ObjectDefinition var =
                new ObjectDefinition(originalCode, "    ", "\n");
            Assert.Equal(expectedCode, var.Format(ref indents));
            Assert.Equal(expectedIndents, indents);
        }
    }
}
