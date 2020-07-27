﻿using System.Collections.Generic;
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

        public ObjectDefinition(string unformattedCode) : base(unformattedCode)
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
                Global.indentation.Repeat(indents)
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

            MatchCollection matches = Regex.Matches(
                _unformattedCode, pattern, RegexOptions.IgnoreCase
            );
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
            List<string> accessModifiers = new List<string>();
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
                else if (
                    part.ToLower() == "abstract"
                    || part.ToLower() == "final"
                    || part.ToLower() == "internal"
                    || part.ToLower() == "public")
                {
                    accessModifiers.Add(part);
                }
            }
            string name = splitDefinition[1 + accessModifiers.Count];

            return new TcObject(
                objectType: "FUNCTION_BLOCK",
                accessModifier: string.Join(" ", accessModifiers.ToArray()),
                name: name,
                dataType: "",
                extends: string.Join(", ", parents.ToArray()),
                implements: string.Join(", ", interfaces.ToArray())
            );
        }

        private TcObject TokenizeMethodOrProperty()
        {
            string entityType = @"\s*(FUNCTION|METHOD|PROPERTY)\s*";
            string accessModifier =
                @"(PRIVATE|PUBLIC|PROTECTED|INTERNAL)?(?:(?: *)(FINAL|ABSTRACT))?\s*";
            string name = @"(\w+)\s*:?";
            string dataType = @"\s*(.*[^\s+;])?";

            string pattern = $@"{entityType}{accessModifier}{name}{dataType}";

            MatchCollection matches = Regex.Matches(_unformattedCode, pattern);
            if (matches.Count > 0)
            {
                Match match = matches[0];
                bool twoModifers = match.Groups[2].Value.Length > 0
                                && match.Groups[3].Value.Length > 0;
                return new TcObject(
                    objectType: match.Groups[1].Value,
                    accessModifier: match.Groups[2].Value
                        + (twoModifers ? " " : "")
                        + match.Groups[3].Value,
                    name: match.Groups[4].Value,
                    dataType: Keywords.Upper(match.Groups[5].Value),
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