using System.Text.RegularExpressions;

namespace TcBlackCore
{
    public class VariableDeclaration : CodeLineBase
    {
        public VariableDeclaration(string unformattedCode) : base(unformattedCode)
        {
        }

        public override string Format(ref int indents)
        {
            TcDeclaration tokens = Tokenize();
            string formattedDatatype = (
                InsertSpacesAroundOperators(tokens.DataType)
                .Replace(",", ", ")
            );
                
            string formattedCode = (
                    Globals.indentation.Repeat(indents)
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
            
            string strInitRegex = $@"([""'])(?:(?=(\$?))\2.)*?\1(?=\s*;)";

            Match match = Regex.Match(unformattedCode, strInitRegex);
            string strInit = "";
            if (match.Length > 0)
            {
                strInit = match.Groups[0].Value;
                unformattedCode = Regex.Replace(unformattedCode, strInitRegex, "");
            }

            MatchCollection matches = Regex.Matches(
                unformattedCode,
                pattern, 
               RegexOptions.IgnoreCase
            );
            TcDeclaration variable;
            if (matches.Count > 0)
            {
                match = matches[0];
                if (strInit.Length == 0)
                {
                    strInit = Keywords.Upper(
                        RemoveWhiteSpaceIfPossible(match.Groups[4].Value)
                    );
                }
                variable = new TcDeclaration(
                    name: RemoveWhiteSpaceIfPossible(match.Groups[1].Value),
                    allocation: RemoveWhiteSpaceIfPossible(match.Groups[2].Value),
                    dataType: Keywords.Upper(
                        RemoveWhiteSpaceIfPossible(match.Groups[3].Value)
                    ),
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

        /// <summary>
        /// Removes spaces between square and round brackets, except if it is a string.
        /// </summary>
        /// <param name="str">The string to remove spaces from.</param>
        /// <returns>Cleaned up string.</returns>
        /// <remarks>source: https://stackoverflow.com/a/63486599/6329629 </remarks>
        public static string RemoveWhiteSpaceIfPossible(string str)
        {
            string pattern = (
                "\\s+(?=[^[]*\\])"
                + "|(?<!['\"][^,]*)\\s+(?=[^(]*\\))"
                + "|\\s+(?![^,]*['\"])(?=[^(]*\\))"
            );

            return Regex.Replace(str, pattern, "").Trim();
        }

        /// <summary>
        /// Return string with single spaces around +, -, * and / operators. 
        /// </summary>
        /// <example>
        /// "a+b" => "a + b"
        /// </example>
        private static string InsertSpacesAroundOperators(string unformatted)
        {
            return Regex.Replace(unformatted, @"(?<=\w|\))([-+\/*])", " $1 ");
        }
    }
}