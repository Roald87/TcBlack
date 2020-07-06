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
            string formattedDatatype = (
                InsertSpacesAroundOperators(tokens.DataType)
                .Replace(",", ", ")
            );
                
            string formattedCode = (
                    _singleIndent.Repeat(indents)
                    + tokens.Name
                    + (tokens.Allocation.Length > 0 ? $" AT {tokens.Allocation}" : "")
                    + $" : {formattedDatatype}"
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
            string variable_pattern = @"^\s*(\w+)\s*";
            string address_pattern = @"(?:AT\s+)?([\w+%.*]*)?\s*";
            string array_pattern = @"ARRAY\[.*\]\s+OF\s+[\w.]+";
            string unit_pattern =
                $@"({array_pattern}\(.*\)|{array_pattern}\[.*\]|{array_pattern}"
                + @"|\w+\(.*\)|\w+\[.*\]|[^;:]*)\s*"; 
            string initialization = $@"(?::=)?(?s)\s*(.*?)?";
            string comment = $@"\s*(\/\/[^\n]+|\(\*.*?\*\))?";
            string pattern = 
                $@"{variable_pattern}{address_pattern}:\s*"
                + $@"{unit_pattern}{initialization};{comment}";
            
            string strInitialization = $@"([""'])(?:(?=(\$?))\2.)*?\1(?=\s*;)";

            Match match = Regex.Match(_unformattedCode, strInitialization);
            string strInit = "";
            if (match.Length > 0)
            {
                strInit = match.Groups[0].Value;
                _unformattedCode = Regex.Replace(_unformattedCode, strInitialization, "");
            }

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            TcDeclaration variable;
            if (matches.Count > 0)
            {
                match = matches[0];
                if (strInit.Length == 0)
                {
                    strInit = RemoveWhiteSpaceIfPossible(match.Groups[4].Value);
                }
                variable = new TcDeclaration(
                    name: RemoveWhiteSpaceIfPossible(match.Groups[1].Value),
                    allocation: RemoveWhiteSpaceIfPossible(match.Groups[2].Value),
                    dataType: RemoveWhiteSpaceIfPossible(match.Groups[3].Value),
                    initialization: strInit,
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

        /// <summary>
        /// Return string with single spaces around the operators. 
        /// </summary>
        /// <example>
        /// "a+b" => "a + b"
        /// </example>
        private string InsertSpacesAroundOperators(string unformatted)
        {
            string formatted = unformatted
                .Replace("+", " + ")
                .Replace("-", " - ")
                .Replace("/", " / ")
                .Replace("*", " * ");

            return formatted;
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