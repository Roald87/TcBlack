using System.Text.RegularExpressions;

namespace TcBlack
{
    public class FunctionDefinition : StatementBase
    {
        public struct TcProgrammingEntity
        {
            public TcProgrammingEntity(
                string entityType,
                string name,
                string dataType
            )
            {
                EntityType = entityType;
                Name = name;
                DataType = dataType;
            }

            public string EntityType { get; }
            public string Name { get; }
            public string DataType { get; }
        }

        public FunctionDefinition(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            TcProgrammingEntity tokens = Tokenize();

            string formattedCode =
                _singleIndent.Repeat(indents) 
                + tokens.EntityType
                + $" {tokens.Name}"
                + $" : {tokens.DataType}";

            return formattedCode;
        }

        private TcProgrammingEntity Tokenize()
        {
            string entityType = @"(?:\s+)?(FUNCTION\w*|METHOD)(?:\s+)";
            string name = @"(\w+)(?:\s+)?(?::)";
            string dataType = @"(?:\s+)?(\w+)";

            string pattern = $@"{entityType}{name}{dataType}";

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            TcProgrammingEntity functionDefinition;
            if (matches.Count > 0)
            {
                Match match = matches[0];
                functionDefinition = new TcProgrammingEntity(
                    entityType: match.Groups[1].Value,
                    name: match.Groups[2].Value,
                    dataType: match.Groups[3].Value
                );
            }
            else
            {
                functionDefinition = new TcProgrammingEntity("", "", "");
            }

            return functionDefinition;
        }
    }
}