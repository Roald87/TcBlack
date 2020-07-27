﻿using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class CompositeCodeTests
    {
        [Fact]
        public void FormatDeclaration()
        {
            Global.indentation = "    ";
            Global.lineEnding = "\n";
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

            uint indents = 0;
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
            Global.indentation = "    ";
            Global.lineEnding = "\n";
            string unformattedCode = "";
            uint indents = 0;
            string actual = new CompositeCode(unformattedCode: unformattedCode)
            .Tokenize()
            .Format(ref indents);

            string expected = "";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RemoveEmptyVariableTypeDeclarations()
        {
            Global.indentation = "    ";
            Global.lineEnding = "\n";
            string unformattedCode =
                "FUNCTION_BLOCK Something\n"
                + "VAR_INPUT\n"
                + "END_VAR\n"
                + "VAR_OUTPUT\n"
                + "END_VAR\n"
                + "VAR\n"
                + "    isThatTrue : BOOL;\n"
                + "END_VAR";
            uint indents = 0;
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
        public void CommentWithoutASpace()
        {
            Global.indentation = "    ";
            Global.lineEnding = "\n";
            string unformattedCode = "    //Some : FB_Some;\n";
            uint indents = 0;
            string actual =
                new CompositeCode(unformattedCode: unformattedCode)
                .Tokenize()
                .Format(ref indents);

            string expected = "    //Some : FB_Some;\n";
            Assert.Equal(expected, actual);
        }
    }
}
