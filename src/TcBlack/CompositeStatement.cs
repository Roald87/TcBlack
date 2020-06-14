using System;
using System.Collections.Generic;

namespace TcBlack
{
    public class CompositeStatement : StatementBase, IStatementOperations
    {
        private List<StatementBase> statements;

        public CompositeStatement(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
            statements = new List<StatementBase>();
        }

        public void Add(StatementBase statement)
        {
            statements.Add(statement);
        }

        public override string Format(ref uint indents)
        {
            string formattedString = "";

            foreach (StatementBase statement in statements)
            {
                formattedString += statement.Format(ref indents) + _lineEnding;
            }

            return formattedString;
        }

        public CompositeStatement Tokenize()
        {
            string[] lines = _unformattedCode.Split(
                new[] { _lineEnding }, StringSplitOptions.None
            );
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    Add(new EmptyLine(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else if (line.StartsWith("END_VAR"))
                {
                    Add(new VariableBlockEnd(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else if (line.StartsWith("VAR"))
                {
                    Add(new VariableBlockStart(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else if (line.StartsWith("FUNCTION") || line.StartsWith("METHOD"))
                {
                    Add(new ObjectDefinition(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else
                {
                    Add(new VariableDeclaration(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
            }

            return this;
        }
    }
}