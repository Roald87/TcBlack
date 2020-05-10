using System;
using System.Collections.Generic;

namespace TcBlack
{
    public class StatementAssigner
    {
        private readonly string _unformattedCode;
        private readonly string _singleIndent;
        private readonly string _lineEnding;

        public StatementAssigner(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        )
        {
            _unformattedCode = unformattedCode;
            _singleIndent = singleIndent;
            _lineEnding = lineEnding;
        }

        public List<StatementBase> Tokenize()
        {
            List<StatementBase> statements = new List<StatementBase>();
            string[] lines = _unformattedCode.Split(
                new[] {_lineEnding}, StringSplitOptions.None
            );
            foreach (string line in lines)
            {
                StatementBase statement;
                if (line.StartsWith("END_VAR"))
                {
                    statement = new VariableBlockEnd(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    );
                }
                else if (line.StartsWith("VAR"))
                {
                    statement = new VariableBlockStart(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    );
                }
                else if (line.StartsWith("FUNCTION") || line.StartsWith("METHOD"))
                {
                    statement = new FunctionDefinition(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    );
                }
                else
                {
                    statement = new VariableDeclaration(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    );
                }

                statements.Add(statement);
            }

            return statements;
        }
    }
}
