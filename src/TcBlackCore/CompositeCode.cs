using System;
using System.Collections.Generic;
using System.Linq;

namespace TcBlackCore
{
    public class CompositeCode : CodeLineBase
    {
        private List<CodeLineBase> codeLines;

        public CompositeCode(string unformattedCode) : base(unformattedCode)
        {
            codeLines = new List<CodeLineBase>();
        }

        /// <summary>
        /// Adds a new code line to the list.
        /// </summary>
        /// <param name="codeLine">The code line to add.</param>
        public void Add(CodeLineBase codeLine)
        {
            codeLines.Add(codeLine);
        }

        /// <summary>
        /// Format all the code.
        /// </summary>
        /// <param name="indents">The number of indents.</param>
        /// <returns>The formatted code.</returns>
        public override string Format(ref int indents)
        {
            string formattedString = "";

            foreach (CodeLineBase codeLine in codeLines)
            {
                formattedString += codeLine.Format(ref indents) + Globals.lineEnding;
            }

            return formattedString;
        }

        /// <summary>
        /// Assigns a specific type to each line.
        /// </summary>
        /// <returns>The CompositeStatement class itself.</returns>
        public CompositeCode Tokenize()
        {
            string lineEndingOfFile = 
                unformattedCode.Contains("\r\n") ? "\r\n" : "\n";
            string[] lines = unformattedCode.Split(
                new[] { lineEndingOfFile }, StringSplitOptions.None
            );
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    if (
                        codeLines.Count > 0 
                        && (codeLines.Last() is EmptyLine 
                        || codeLines.Last() is VariableBlockStart)
                    )
                    {
                        continue;
                    }
                    Add(new EmptyLine(unformattedCode: line));
                }
                else if (
                    line.StartsWith("END_VAR", StringComparison.OrdinalIgnoreCase)
                )
                {
                    if (codeLines.Last() is VariableBlockStart)
                    {
                        codeLines.RemoveAt(codeLines.Count - 1);
                    }
                    else if (codeLines.Last() is EmptyLine)
                    {
                        codeLines.RemoveAt(codeLines.Count - 1);
                        Add(new VariableBlockEnd(unformattedCode: line));
                    }
                    else
                    {
                        Add(new VariableBlockEnd(unformattedCode: line));
                    }
                }
                else if (IsVariableBlockStart(line))
                {
                    TryRemoveLastEmptyLine();
                    Add(new VariableBlockStart(unformattedCode: line));
                }
                else if (
                    line.StartsWith("FUNCTION", StringComparison.OrdinalIgnoreCase) 
                    || line.StartsWith("METHOD", StringComparison.OrdinalIgnoreCase) 
                    || line.StartsWith("PROPERTY", StringComparison.OrdinalIgnoreCase)
                    || line.StartsWith("INTERFACE", StringComparison.OrdinalIgnoreCase))
                {
                    Add(new ObjectDefinition(unformattedCode: line));
                }
                else if (LooksLikeVariableDeclaration(line))
                {
                    Add(new VariableDeclaration(unformattedCode: line));
                }
                else
                {
                    Add(new UnknownCodeType(unformattedCode: line));
                }
            }

            RemoveAllEmptyLinesAtTheEnd();

            return this;
        }

        static private bool IsVariableBlockStart(string text)
        {
            string trimmedText = text.Trim().ToUpperInvariant();

            return 
                trimmedText == "VAR" 
                || trimmedText.StartsWith("VAR_", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes the last empty line from the list if it exists.
        /// </summary>
        private void TryRemoveLastEmptyLine()
        {
            try
            {
                if (codeLines.Last() is EmptyLine)
                {
                    codeLines.RemoveAt(codeLines.Count - 1);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Removes all the empty lines which are in the end of the statement list.
        /// </summary>
        private void RemoveAllEmptyLinesAtTheEnd()
        {
            for (int i = codeLines.Count - 1; i >= 0; i--)
            {
                if (codeLines[i] is EmptyLine)
                {
                    codeLines.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Uses a regex pattern match in Tokenize to check if the code looks like a 
        /// declaration.
        /// </summary>
        /// <param name="codeLine">Code line to inspect</param>
        /// <returns>Returns true if it thinks the code is a declaration.</returns>
        static private bool LooksLikeVariableDeclaration(string codeLine)
        {
            var code = new VariableDeclaration(codeLine).Tokenize();

            return !string.IsNullOrEmpty(code.Name);
        }
    }
}