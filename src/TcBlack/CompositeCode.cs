using System;
using System.Collections.Generic;
using System.Linq;

namespace TcBlack
{
    public class CompositeCode : CodeLineBase, ICodeLineOperations
    {
        private List<CodeLineBase> codeLines;

        public CompositeCode(
            string unformattedCode,
            string singleIndent,
            string lineEnding
        ) : base(unformattedCode, singleIndent, lineEnding)
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
        public override string Format(ref uint indents)
        {
            string formattedString = "";

            foreach (CodeLineBase codeLine in codeLines)
            {
                formattedString += codeLine.Format(ref indents) + _lineEnding;
            }

            return formattedString;
        }

        /// <summary>
        /// Assigns a specific type to each line.
        /// </summary>
        /// <returns>The CompositeStatement class itself.</returns>
        public CompositeCode Tokenize()
        {
            string[] lines = _unformattedCode.Split(
                new[] { _lineEnding }, StringSplitOptions.None
            );
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    if (codeLines.Count > 0 && codeLines.Last() is EmptyLine)
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
                    if (codeLines.Last() is VariableBlockStart)
                    {
                        codeLines.RemoveAt(codeLines.Count - 1);
                    }
                    else
                    {
                        Add(new VariableBlockEnd(
                            unformattedCode: line,
                            singleIndent: _singleIndent,
                            lineEnding: _lineEnding
                        ));
                    }
                }
                else if (line.StartsWith("VAR"))
                {
                    Add(new VariableBlockStart(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else if (
                    line.StartsWith("FUNCTION") 
                    || line.StartsWith("METHOD") 
                    || line.StartsWith("PROPERTY")
                    || line.StartsWith("INTERFACE"))
                {
                    Add(new ObjectDefinition(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else if (LooksLikeVariableDeclaration(line))
                {
                    Add(new VariableDeclaration(
                        unformattedCode: line,
                        singleIndent: _singleIndent,
                        lineEnding: _lineEnding
                    ));
                }
                else
                {
                    Add(new UnknownCodeType(
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
        private bool LooksLikeVariableDeclaration(string codeLine)
        {
            var code = new VariableDeclaration(codeLine, _singleIndent, _lineEnding)
                .Tokenize();

            return code.Name != "";
        }
    }
}