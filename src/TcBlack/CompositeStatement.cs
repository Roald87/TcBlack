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
    }
}