using System.Collections.Generic;
using Xunit;
using TcBlack;

namespace TcBlackTests
{
    public class StatementAssignerTests
    {
        private readonly string lineEnding = "\n";
        private readonly string singleIndent = "    ";

        [Fact]
        public void DetectSingleBlockStart()
        {
            string code = "VAR_INPUT";
            StatementAssigner assigner = new StatementAssigner(
                unformattedCode: code, 
                singleIndent: singleIndent, 
                lineEnding: lineEnding
            );
            List<StatementBase> actual = assigner.Tokenize();
            List<StatementBase> expected = new List<StatementBase>();

            Assert.IsType<VariableBlockStart>(actual[0]);
            Assert.Equal($"{code}\n", FormatStatements(actual));
        }

        [Fact]
        public void DetectSingleLineOfCode()
        {
            string code = "var1 : BOOL;";
            StatementAssigner assigner = new StatementAssigner(
                unformattedCode: code,
                singleIndent: singleIndent,
                lineEnding: lineEnding
            );
            List<StatementBase> actual = assigner.Tokenize();

            Assert.IsType<VariableDeclaration>(actual[0]);
            Assert.Equal($"{code}\n", FormatStatements(actual));
        }

        [Fact]
        public void DetectSingleBlockEnd()
        {
            string code = "END_VAR";
            StatementAssigner assigner = new StatementAssigner(
                unformattedCode: code,
                singleIndent: singleIndent,
                lineEnding: lineEnding
            );
            List<StatementBase> actual = assigner.Tokenize();

            Assert.IsType<VariableBlockEnd>(actual[0]);
            Assert.Equal($"{code}\n", FormatStatements(actual));
        }

        [Theory]
        [InlineData("FUNCTION Sum : LREAL")]
        [InlineData("FUNCTION_BLOCK Sum : LREAL")]
        [InlineData("METHOD Sum : LREAL")]
        public void DetectSingleFunctionDefinition(string code)
        {
            StatementAssigner assigner = new StatementAssigner(
                unformattedCode: code,
                singleIndent: singleIndent,
                lineEnding: lineEnding
            );
            List<StatementBase> actual = assigner.Tokenize();

            Assert.IsType<ObjectDefinition>(actual[0]);
            Assert.Equal($"{code}\n", FormatStatements(actual));
        }

        [Theory]
        [InlineData("")]
        public void DetectEmptyLine(string code)
        {
            StatementAssigner assigner = new StatementAssigner(
                unformattedCode: code,
                singleIndent: singleIndent,
                lineEnding: lineEnding
            );
            List<StatementBase> actual = assigner.Tokenize();
            Assert.IsType<EmptyLine>(actual[0]);
            Assert.Equal(lineEnding, FormatStatements(actual));
        }

        [Fact]
        public void DetectBlockStartEndAndLineOfCode()
        {
            string code = 
                "VAR_INPUT\n"
                + "var1:BOOL;\n"
                + "END_VAR";
            StatementAssigner assigner = new StatementAssigner(
                unformattedCode: code,
                singleIndent: singleIndent,
                lineEnding: lineEnding
            );
            List<StatementBase> actual = assigner.Tokenize();
            string expected =
                "VAR_INPUT\n"
                + "    var1 : BOOL;\n"
                + "END_VAR\n";

            Assert.Equal(3, actual.Count);
            Assert.IsType<VariableBlockStart>(actual[0]);
            Assert.IsType<VariableDeclaration>(actual[1]);
            Assert.IsType<VariableBlockEnd>(actual[2]);
            Assert.Equal(expected, FormatStatements(actual));
        }

        public string FormatStatements(List<StatementBase> statements)
        {
            CompositeStatement codeBlock = new CompositeStatement(
                "Code block", 
                singleIndent, 
                lineEnding
            );
            foreach (var statement in statements)
            {
                codeBlock.Add(statement);
            }

            uint indents = 0;
            return codeBlock.Format(ref indents);
        }
    }
}
