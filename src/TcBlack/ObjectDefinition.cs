using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TcBlack
{
    /// <summary>
    /// Format the definition of a FUNCTION, FUNCTION_BLOCK, etc. including the 
    /// IMPLEMENTS and EXTENDS.
    /// </summary>
    public class ObjectDefinition : CodeLineBase
    {
        public struct TcObject
        {
            public TcObject(
                string objectType,
                string accessModifier,
                string name,
                string dataType,
                string implements,
                string extends
            )
            {
                ObjectType = objectType;
                AccessModifier = accessModifier;
                Name = name;
                DataType = dataType;
                Extends = extends;
                Implements = implements;
            }

            public string ObjectType { get; }
            public string AccessModifier { get; }
            public string Name { get; }
            public string DataType { get; }
            public string Extends { get; }
            public string Implements { get; }
        }

        public ObjectDefinition(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
        {
        }

        /// <summary>
        /// Return the formatted code.
        /// </summary>
        /// <param name="indents">Number of indents to place in front.</param>
        /// <returns>Formatted code.</returns>
        public override string Format(ref uint indents)
        {
            TcObject tokens = Tokenize();

            string formattedCode =
                _singleIndent.Repeat(indents)
                + tokens.ObjectType
                + (tokens.AccessModifier.Length > 0 ? $" {tokens.AccessModifier}" : "")
                + $" {tokens.Name}"
                + (tokens.DataType.Length > 0 ? $" : {tokens.DataType}" : "")
                + (tokens.Extends.Length > 0 ?
                    $" EXTENDS {tokens.Extends}" : "")
                + (tokens.Implements.Length > 0 ?
                    $" IMPLEMENTS {tokens.Implements}" : "");

            return formattedCode;
        }

        /// <summary>
        /// Return the split object definition. 
        /// </summary>
        /// <returns>The split object defination.</returns>
        private TcObject Tokenize()
        {
            if (_unformattedCode.Contains("FUNCTION_BLOCK"))
            {
                return TokenizeFunctionBlock();
            }
            else if (_unformattedCode.Contains("INTERFACE"))
            {
                return TokenizeInterface();
            }
            else
            {
                return TokenizeMethodOrProperty();
            }
        }

        private TcObject TokenizeInterface()
        {
            string pattern = @"INTERFACE\s+(\w+)\s*(?:EXTENDS((?:[\s,]+[\w\.]+)+))?";

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            if (matches.Count > 0)
            {
                Match match = matches[0];
                string[] parents = Regex.Split(match.Groups[2].Value, @"[\s,]+");
                return new TcObject(
                    objectType: "INTERFACE",
                    accessModifier: "",
                    name: match.Groups[1].Value,
                    dataType: "",
                    extends: string.Join(", ", parents.Skip(1)),
                    implements: ""
                );
            }
            else
            {
                return new TcObject("", "", "", "", "", "");
            }
        }

        private TcObject TokenizeFunctionBlock()
        {
            string[] splitDefinition = Regex
                .Split(
                    _unformattedCode,
                    @",|\s+",
                    RegexOptions.IgnorePatternWhitespace
                )
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            List<string> interfaces = new List<string>();
            List<string> parents = new List<string>();
            bool implements = false;
            bool extends = false;
            foreach (string part in splitDefinition)
            {
                bool implementsStarts = (part.ToLower() == "implements");
                bool extendsStarts = (part.ToLower() == "extends");

                if (implements && !extendsStarts)
                {
                    interfaces.Add(part);
                }
                if (extends && !implementsStarts)
                {
                    parents.Add(part);
                }

                if (part.ToLower() == "implements")
                {
                    implements = true;
                    extends = false;
                }
                else if (part.ToLower() == "extends")
                {
                    extends = true;
                    implements = false;
                }
            }

            return new TcObject(
                objectType: "FUNCTION_BLOCK",
                accessModifier: "",
                name: splitDefinition[1],
                dataType: "",
                extends: string.Join(", ", parents.ToArray()),
                implements: string.Join(", ", interfaces.ToArray())
            );
        }

        private TcObject TokenizeMethodOrProperty()
        {
            string entityType = @"\s*(FUNCTION|METHOD|PROPERTY)\s*";
            string accessModifier = @"(PRIVATE|PUBLIC|PROTECTED|INTERNAL)?\s*";
            string name = @"(\w+)\s*:?";
            string dataType = @"\s*(.*[^\s+;])?";

            string pattern = $@"{entityType}{accessModifier}{name}{dataType}";

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            if (matches.Count > 0)
            {
                Match match = matches[0];
                return new TcObject(
                    objectType: match.Groups[1].Value,
                    accessModifier: match.Groups[2].Value,
                    name: match.Groups[3].Value,
                    dataType: match.Groups[4].Value,
                    extends: "",
                    implements: ""
                );
            }
            else
            {
                return new TcObject("", "", "", "", "", "");
            }
        }
    }
}