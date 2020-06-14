using System.Text.RegularExpressions;

namespace TcBlack
{
    public class ObjectDefinition : CodeLineBase
    {
        public struct TcObject
        {
            public TcObject(
                string objectType,
                string name,
                string dataType
            )
            {
                EntityType = objectType;
                Name = name;
                DataType = dataType;
            }

            public string EntityType { get; }
            public string Name { get; }
            public string DataType { get; }
        }

        public ObjectDefinition(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            TcObject tokens = Tokenize();

            string formattedCode =
                _singleIndent.Repeat(indents) 
                + tokens.EntityType
                + $" {tokens.Name}"
                + (tokens.DataType.Length > 0 ? $" : {tokens.DataType}" : "");

            return formattedCode;
        }

        private TcObject Tokenize()
        {
            string entityType = @"(?:\s+)?(FUNCTION\w*|METHOD)(?:\s+)";
            string name = @"(\w+)(?:\s+)?(?::)?";
            string dataType = @"(?:\s+)?(\w+)?";

            string pattern = $@"{entityType}{name}{dataType}";

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            TcObject functionDefinition;
            if (matches.Count > 0)
            {
                Match match = matches[0];
                functionDefinition = new TcObject(
                    objectType: match.Groups[1].Value,
                    name: match.Groups[2].Value,
                    dataType: match.Groups[3].Value
                );
            }
            else
            {
                functionDefinition = new TcObject("", "", "");
            }

            return functionDefinition;
        }
    }
}