using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Adds a new statement (code line) to the list.
        /// </summary>
        /// <param name="statement">The code line to add.</param>
        public void Add(StatementBase statement)
        {
            statements.Add(statement);
        }

        /// <summary>
        /// Format all the code.
        /// </summary>
        /// <param name="indents">The number of indents.</param>
        /// <returns>The formatted code.</returns>
        public override string Format(ref uint indents)
        {
            string formattedString = "";

            foreach (StatementBase statement in statements)
            {
                formattedString += statement.Format(ref indents) + _lineEnding;
            }

            return formattedString;
        }

        /// <summary>
        /// Assigns a specific type to each line.
        /// </summary>
        /// <returns>The CompositeStatement class itself.</returns>
        public CompositeStatement Tokenize()
        {
            string[] lines = _unformattedCode.Split(
                new[] { _lineEnding }, StringSplitOptions.None
            );
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    if (statements.Last() is EmptyLine)
                    {
                        continue;
                    }
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

            RemoveAllEmptyLinesAtTheEnd();

            return this;
        }

        /// <summary>
        /// Removes all the empty lines which are in the end of the statement list.
        /// </summary>
        private void RemoveAllEmptyLinesAtTheEnd()
        {
            for (int i = statements.Count - 1; i >= 0; i--)
            {
                if (statements[i] is EmptyLine)
                {
                    statements.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }
    }
}