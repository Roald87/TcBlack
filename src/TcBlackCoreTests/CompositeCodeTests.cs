using System;
using TcBlackCore;
using Xunit;

[assembly: CLSCompliant(false)]
namespace TcBlackTests
{
    [Collection("Sequential")]
    public class CompositeCodeTests
    {
        [Fact]
        public void FormatDeclaration()
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            string unformattedCode =
                "// Single line comments are also not formatted, yet\n" 
                + "FUNCTION AddIntegers:DINT\n"
                + "VAR_INPUT\n"
                + "var1:      LREAL  :=9.81 ;      // Comment\n"
                + "var2  AT %Q*:  BOOL     ;   \n"
                + "\n\n\n"
                + "    {attribute 'hide'}\n"
                + "anotherBool : BOOL:=TRUE;\n"
                + "END_VAR\n\n\n";
            CompositeCode statements = 
                new CompositeCode(unformattedCode: unformattedCode).Tokenize();

            int indents = 0;
            string actual = statements.Format(ref indents);
            string expected =
                "// Single line comments are also not formatted, yet\n"
                + "FUNCTION AddIntegers : DINT\n"
                + "VAR_INPUT\n"
                + "    var1 : LREAL := 9.81; // Comment\n"
                + "    var2 AT %Q* : BOOL;\n"
                + "    \n"
                + "    {attribute 'hide'}\n"
                + "    anotherBool : BOOL := TRUE;\n"
                + "END_VAR\n";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatEmptyDeclaration()
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            string unformattedCode = "";
            int indents = 0;
            string actual = 
                new CompositeCode(unformattedCode: unformattedCode)
                .Tokenize()
                .Format(ref indents);

            string expected = "";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveEmptyVariableTypeDeclarations()
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            string unformattedCode =
                "FUNCTION_BLOCK Something\n"
                + "VAR_INPUT\n"
                + "END_VAR\n"
                + "VAR_OUTPUT\n"
                + "END_VAR\n"
                + "VAR\n"
                + "    isThatTrue : BOOL;\n"
                + "END_VAR";
            int indents = 0;
            string actual = 
                new CompositeCode(unformattedCode: unformattedCode)
                .Tokenize()
                .Format(ref indents);

            string expected = 
                "FUNCTION_BLOCK Something\n"
                + "VAR\n"
                + "    isThatTrue : BOOL;\n"
                + "END_VAR\n";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveEmptyLinesBeforeAndAfterVarBlockStartAndEnd()
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            string unformattedCode =
                "FUNCTION Abx\n"
                + "\n" 
                + "VAR_INPUT\n"
                + "\n"
                + "    number : INT;\n"
                + "\n\n"
                + "END_VAR\n"
                + "\n"
                + "VAR\n"
                + "\n\n"
                + "    someVar : BOOL;\n"
                + "\n"
                + "END_VAR\n";
            int indents = 0;
            string actual =
                new CompositeCode(unformattedCode: unformattedCode)
                .Tokenize()
                .Format(ref indents);

            string expected =
                "FUNCTION Abx\n"
                + "VAR_INPUT\n"
                + "    number : INT;\n"
                + "END_VAR\n"
                + "VAR\n"
                + "    someVar : BOOL;\n"
                + "END_VAR\n";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CommentWithoutASpace()
        {
            Globals.indentation = "    ";
            Globals.lineEnding = "\n";
            string unformattedCode = "    //Some : FB_Some;\n";
            int indents = 0;
            string actual =
                new CompositeCode(unformattedCode: unformattedCode)
                .Tokenize()
                .Format(ref indents);

            string expected = "    //Some : FB_Some;\n";
            Assert.Equal(expected, actual);
        }
    }
}
