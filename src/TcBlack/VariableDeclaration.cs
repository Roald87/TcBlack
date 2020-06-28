using System.Text;
using System.Text.RegularExpressions;

namespace TcBlack
{
    public struct TcDeclaration
    {
        public TcDeclaration(
            string name,
            string allocation,
            string dataType,
            string initialization,
            string comment
        )
        {
            Name = name;
            Allocation = allocation;
            DataType = dataType;
            Initialization = initialization;
            Comment = comment;
        }

        public string Name { get; }
        public string Allocation { get; }
        public string DataType { get; }
        public string Initialization { get; }
        public string Comment { get; }
    }

    public class VariableDeclaration : CodeLineBase
    {
        public VariableDeclaration(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        public override string Format(ref uint indents)
        {
            TcDeclaration tokens = Tokenize();

            string formattedCode = (
                    _singleIndent.Repeat(indents)
                    + tokens.Name
                    + (tokens.Allocation.Length > 0 ? $" AT {tokens.Allocation}" : "")
                    + $" : {tokens.DataType.Replace(",", ", ")}"
                    + (tokens.Initialization.Length > 0 ? 
                        $" := {tokens.Initialization.Replace(",", ", ")}" : ""
                    )
                    + ";"
                    + (tokens.Comment.Length > 0 ? $" {tokens.Comment}" : "")
                );

            return formattedCode;
        }

        public TcDeclaration Tokenize()
        {
            string variable_pattern = @"(\w+)";
            string possible_space = @"(?:\s+)?";
            string address_pattern = @"(?:AT\s+)?([\w+%.*]*)?";
            string array_pattern = @"ARRAY\[.*\]\s+OF\s+[\w.]+";
            string unit_pattern =
                $@"({array_pattern}\(.*\)|{array_pattern}\[.*\]|{array_pattern}"
                + @"|\w+\(.*\)|\w+\[.*\]|[^;:]*)"; 
            string initialization = $@"(?::=)?(?s){possible_space}(.*?)?";
            string comment = $@"{possible_space}(\/\/[^\n]+|\(\*.*?\*\))?";
            string pattern = 
                $@"{variable_pattern}{possible_space}"
                + $@"{address_pattern}{possible_space}(?::){possible_space}"
                + $@"{unit_pattern}{possible_space}{initialization}(?:;){comment}";

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            TcDeclaration variable;
            if (matches.Count > 0)
            {
                Match match = matches[0];
                variable = new TcDeclaration(
                    name: RemoveWhiteSpaceIfPossible(match.Groups[1].Value),
                    allocation: RemoveWhiteSpaceIfPossible(match.Groups[2].Value),
                    dataType: RemoveWhiteSpaceIfPossible(match.Groups[3].Value),
                    initialization: RemoveWhiteSpaceIfPossible(match.Groups[4].Value),
                    comment: match.Groups[5].Value.Trim()
                );
            }
            else
            {
                variable = new TcDeclaration("", "", "", "", "");
            }

            return variable;
        }

        public string RemoveWhiteSpaceIfPossible(string str)
        {
            string pattern = @"\s+(?=[^[\]]*\])|\s+(?=[^()]*\))";

            return Regex.Replace(str, pattern, "").Trim();
        }
    }

    /// <summary>
    /// Source: https://stackoverflow.com/a/47915552/6329629
    /// </summary>
    public static class StringExtensions
    {
        public static string Repeat(this string s, uint n)
        {
            string _repeatedString = new StringBuilder(s.Length * (int)n)
                .Insert(0, s, (int)n)
                .ToString();

            return _repeatedString;
        }
    }
}