using TcBlackCore;
using Xunit;

namespace TcBlackTests
{
    [Collection("Sequential")]
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
            int indents,
            string expectedCode,
            int expectedIndents
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
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
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
            int indents = 0;
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
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
            int indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "FUNCTION_BLOCK    FINAL Sum IMPLEMENTS   Interface",
            "FUNCTION_BLOCK FINAL Sum IMPLEMENTS Interface"
        )]
        [InlineData(
            "FUNCTION_BLOCK    ABSTRACT     Sum IMPLEMENTS Interface, interface2",
            "FUNCTION_BLOCK ABSTRACT Sum IMPLEMENTS Interface, interface2"
        )]
        [InlineData(
            "FUNCTION_BLOCK INTERNAL  FINAL Sum EXTENDS FB_SumParent IMPLEMENTS  I_ab",
            "FUNCTION_BLOCK INTERNAL FINAL Sum EXTENDS FB_SumParent IMPLEMENTS I_ab"
        )]
        [InlineData(
            "FUNCTION_BLOCK  PUBLIC  ABSTRACT     Sum IMPLEMENTS Interface, interface2",
            "FUNCTION_BLOCK PUBLIC ABSTRACT Sum IMPLEMENTS Interface, interface2"
        )]
        [InlineData(
            "  PROPERTY PUBLIC ABSTRACT  Test:BOOL",
            "PROPERTY PUBLIC ABSTRACT Test : BOOL"
        )]
        [InlineData(
            "  PROPERTY  FINAL  Test:BOOL",
            "PROPERTY FINAL Test : BOOL"
        )]
        [InlineData(
            "METHOD  PROTECTED FINAL Sum : BOOL",
            "METHOD PROTECTED FINAL Sum : BOOL"
        )]
        [InlineData(
            "METHOD     PROTECTED ABSTRACT Sum : BOOL",
            "METHOD PROTECTED ABSTRACT Sum : BOOL"
        )]
        [InlineData(
            "METHOD     PRIVATE ABSTRACT Sum : BOOL",
            "METHOD PRIVATE ABSTRACT Sum : BOOL"
        )]
        [InlineData(
            "METHOD     INTERNAL ABSTRACT Sum : BOOL",
            "METHOD INTERNAL ABSTRACT Sum : BOOL"
        )]
        [InlineData(
            "METHOD  PROTECTED   FINAL Sum : BOOL",
            "METHOD PROTECTED FINAL Sum : BOOL"
        )]
        [InlineData(
            "METHOD   FINAL Sum : BOOL",
            "METHOD FINAL Sum : BOOL"
        )]
        [InlineData(
            "METHOD  PROTECTED    Sum : BOOL",
            "METHOD PROTECTED Sum : BOOL"
        )]
        public void AbstractAndFinalModifiersForMethodsAndFunctionBlocks(
            string originalCode, string expectedCode
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
            int indents = 0;
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
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
            int indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }

        [Theory]
        [InlineData(
            "METHOD PUBLIC Close : uint;",
            "METHOD PUBLIC Close : UINT"
        )]
        [InlineData(
            "METHOD Read:word   ",
            "METHOD Read : WORD"
        )]
        [InlineData(
            "METHOD Read    : string(10)    ",
            "METHOD Read : STRING(10)"
        )]
        [InlineData(
            "METHOD Read    : pointer to int    ",
            "METHOD Read : POINTER TO INT"
        )]
        [InlineData(
            "METHOD Read    : pointer to Custom_Type    ",
            "METHOD Read : POINTER TO Custom_Type"
        )]
        [InlineData(
            "METHOD Read    : pointer to string(3456)    ",
            "METHOD Read : POINTER TO STRING(3456)"
        )]
        [InlineData(
            "METHOD Read    : array[1..100] of udint    ",
            "METHOD Read : ARRAY[1..100] OF UDINT"
        )]
        [InlineData(
            "METHOD Read    : pointer to array[1..100] of udint    ",
            "METHOD Read : POINTER TO ARRAY[1..100] OF UDINT"
        )]
        [InlineData(
            "METHOD Read    : pointer to array[1..nNumber] of int    ",
            "METHOD Read : POINTER TO ARRAY[1..nNumber] OF INT"
        )]
        [InlineData(
            "METHOD Read    : String(nInt)    ",
            "METHOD Read : STRING(nInt)"
        )]
        public void MethodsWithStandardReturnTypes(
            string originalCode, string expectedCode
        )
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
            int indents = 0;
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
        [InlineData(
            "INTERFACE ITF_1 EXTENDS I_TcArguments,I_Number2,I_A",
            "INTERFACE ITF_1 EXTENDS I_TcArguments, I_Number2, I_A"
        )]
        [InlineData(
            "INTERFACE I_Test EXTENDS __SYSTEM.IQueryInterface",
            "INTERFACE I_Test EXTENDS __SYSTEM.IQueryInterface"
        )]
        public void FormatInterfaces(string originalCode, string expectedCode)
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            ObjectDefinition var = new ObjectDefinition(originalCode);
            int indents = 0;
            Assert.Equal(expectedCode, var.Format(ref indents));
        }
    }
}
