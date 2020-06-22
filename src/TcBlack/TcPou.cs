using System.Collections.Generic;
using System.Xml;

namespace TcBlack
{
    /// <summary>
    /// Load, format and save a TcPOU file.
    /// </summary>
    public class TcPou
    {
        private XmlDocument doc;
        private readonly string declarationNode = "/TcPlcObject/POU/Declaration";
        private string _path;

        /// <summary>
        /// Loads a TcPOU file.
        /// </summary>
        /// <param name="path">Path to the TcPOU file.</param>
        public TcPou(string path)
        {
            _path = path;
            doc = new XmlDocument();
            doc.Load(path);
        }

        /// <summary>
        /// Format the TwinCAT TcPOU file.
        /// </summary>
        /// <returns>The formatted TcPOU object.</returns>
        public TcPou Format()
        {
            string lineEnding = "\r\n";
            var declarationToFormat = new CompositeCode(
                    Declaration, "    ", lineEnding
                )
                .Tokenize();

            uint indents = 0;
            Declaration = declarationToFormat.Format(ref indents);

            return this;
        }

        /// <summary>
        /// Overwrites the currently saved TcPOU file on disk.
        /// </summary>
        public void Save()
        {
            doc.Save(_path);
        }

        /// <summary>
        /// The TcPOU declaration: VAR/VAR_INPUT/VAR_OUTPUT etc.
        /// </summary>
        public string Declaration
        {
            get => doc.SelectSingleNode(declarationNode).InnerText;
            private set => doc.SelectSingleNode(declarationNode).InnerXml = 
                $"<![CDATA[{value}]]>";
        }
    }
}
