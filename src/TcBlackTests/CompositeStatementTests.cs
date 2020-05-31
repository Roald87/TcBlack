using TcBlack;
using Xunit;

namespace TcBlackTests
{
    public class CompositeStatementTests
    {
        [Fact]
        public void FormatVarInput()
        {
            string singleIndent = "    ";
            CompositeStatement statements = new CompositeStatement(
                unformattedCode:"VAR_INPUT", 
                singleIndent: singleIndent, 
                lineEnding:"\n"
            );

            statements.Add(new ObjectDefinition(
                unformattedCode: "FUNCTION    AddIntegers:DINT",
                singleIndent: singleIndent,
                lineEnding: "\n"
            ));
            statements.Add(new VariableBlockStart(
                    unformattedCode: "VAR_INPUT",
                    singleIndent: singleIndent,
                    lineEnding: "\n"
            ));
            statements.Add(new VariableDeclaration(
                unformattedCode: "var1:      LREAL  :=9.81 ;      // Comment",
                singleIndent: singleIndent,
                lineEnding: "\n"
            ));
            statements.Add(new VariableDeclaration(
                unformattedCode: "var2  AT %Q*:  BOOL     ;   ",
                singleIndent: singleIndent,
                lineEnding: "\n"
            ));
            statements.Add(new VariableBlockEnd(
                unformattedCode: "END_VAR",
                singleIndent: singleIndent,
                lineEnding: "\n"
            ));
            uint indents = 0;
            string actual = statements.Format(ref indents);

            string expected =
                "FUNCTION AddIntegers : DINT\n" 
                + "VAR_INPUT\n" 
                + "    var1 : LREAL := 9.81; // Comment\n"
                + "    var2 AT %Q* : BOOL;\n"
                + "END_VAR\n";

            Assert.Equal(expected, actual);
        } 
    }
}
