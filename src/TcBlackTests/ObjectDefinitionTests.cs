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
        [InlineData("   PROPERTY Adder :LREAL", 2, "        PROPERTY Adder : LREAL", 2)]
        [InlineData(
            "   FUNCTION_BLOCK Subtract", 0, "FUNCTION_BLOCK Subtract", 0
        )]
        public void FormatObjectDefinitionsWithDifferentIndentsAndSpacings(
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

        [Theory]
        [InlineData(
            "FUNCTION_BLOCK Sum IMPLEMENTS   Interface",
            "FUNCTION_BLOCK Sum IMPLEMENTS Interface"
        )]
        [InlineData(
            "FUNCTION_BLOCK Sum IMPLEMENTS   Interface1,   Interface2",
            "FUNCTION_BLOCK Sum IMPLEMENTS Interface1, Interface2"
        )]
        [InlineData(
            "FUNCTION_BLOCK Sum IMPLEMENTS   Interface1     EXTENDS FB_Base",
            "FUNCTION_BLOCK Sum EXTENDS FB_Base IMPLEMENTS Interface1"
        )]
        [InlineData(
            "FUNCTION_BLOCK Sum     EXTENDS FB_Base,FB_Base2,FB_Base3",
            "FUNCTION_BLOCK Sum EXTENDS FB_Base, FB_Base2, FB_Base3"
        )]
        [InlineData(
            "FUNCTION_BLOCK Sum     EXTENDS FB_Base,FB_Base2,FB_Base3\nIMPLEMENTS   Interface1,   Interface2",
            "FUNCTION_BLOCK Sum EXTENDS FB_Base, FB_Base2, FB_Base3 IMPLEMENTS Interface1, Interface2"
        )]
        public void FormatFunctionBlockDefinitionsWithInterfaceAndInherit(
            string originalCode, string expectedCode
        )
        {
            ObjectDefinition var =
                new ObjectDefinition(originalCode, "    ", "\n");
            uint indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }

        [Theory]
        [InlineData("METHOD PRIVATE Sum : BOOL", "METHOD PRIVATE Sum : BOOL")]
        [InlineData("METHOD  PUBLIC   Sum : BOOL", "METHOD PUBLIC Sum : BOOL")]
        [InlineData("METHOD     PROTECTED Sum : BOOL", "METHOD PROTECTED Sum : BOOL")]
        [InlineData("METHOD INTERNAL   Sum : BOOL", "METHOD INTERNAL Sum : BOOL")]
        [InlineData("PROPERTY PRIVATE Test :  BOOL", "PROPERTY PRIVATE Test : BOOL")]
        [InlineData("   PROPERTY PUBLIC Test :  BOOL", "PROPERTY PUBLIC Test : BOOL")]
        [InlineData("PROPERTY PROTECTED Test : BOOL ", "PROPERTY PROTECTED Test : BOOL")]
        [InlineData("  PROPERTY  INTERNAL Test:BOOL", "PROPERTY INTERNAL Test : BOOL")]
        public void MethodsWithVariousAccessModifiersAndWhiteSpaces(
            string originalCode, string expectedCode
        )
        {
            ObjectDefinition var =
                new ObjectDefinition(originalCode, "    ", "\n");
            uint indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "METHOD PUBLIC Close : SysFile.SysTypes.RTS_IEC_RESULT;",
            "METHOD PUBLIC Close : SysFile.SysTypes.RTS_IEC_RESULT"
        )]
        [InlineData(
            "METHOD Read:SysFile.SysTypes.RTS_IEC_RESULT    ",
            "METHOD Read : SysFile.SysTypes.RTS_IEC_RESULT"
        )]
        [InlineData(
            "METHOD Read    : STRING(10)    ",
            "METHOD Read : STRING(10)"
        )]
        public void MethodsWithReturnTypesWithFullPath(
            string originalCode, string expectedCode
        )
        {
            ObjectDefinition var =
                new ObjectDefinition(originalCode, "    ", "\n");
            uint indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "INTERFACE    ITF_1",
            "INTERFACE ITF_1"
        )]
        [InlineData(
            "INTERFACE    ITF_1   EXTENDS    I_TcArguments",
            "INTERFACE ITF_1 EXTENDS I_TcArguments"
        )]
        [InlineData(
            "INTERFACE    ITF_1   EXTENDS    I_TcArguments   ,   I_Number2",
            "INTERFACE ITF_1 EXTENDS I_TcArguments, I_Number2"
        )]
        [InlineData(
            "INTERFACE    ITF_1   EXTENDS    I_TcArguments   ,   I_Number2, I_A",
            "INTERFACE ITF_1 EXTENDS I_TcArguments, I_Number2, I_A"
        )]
        public void FormatInterfaces(
            string originalCode, string expectedCode
        )
        {
            ObjectDefinition var =
                new ObjectDefinition(originalCode, "    ", "\n");
            uint indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }

    }
}
