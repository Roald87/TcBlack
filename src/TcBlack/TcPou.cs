using System.Xml;

namespace TcBlack
{
    /// <summary>
    /// Load, format and save a TcPOU file.
    /// </summary>
    public class TcPou
    {
        private XmlDocument doc;
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

            string text = doc.InnerXml;
            LineEnding = text.Contains("\r\n") ? "\r\n" : "\n";
            Indentation = text.Contains("\t") ? "\t" : "    ";
        }

        /// <summary>
        /// Format the TwinCAT TcPOU file.
        /// </summary>
        /// <returns>The formatted TcPOU object.</returns>
        public TcPou FormatDeclaration()
        {
            uint indents = 0;
            XmlNodeList nodes = doc.SelectNodes(".//Declaration");
            foreach (XmlNode node in nodes)
            {
                string formattedCode = 
                    new CompositeCode(node.InnerText, Indentation, LineEnding)
                    .Tokenize()
                    .Format(ref indents);
                node.InnerXml = $"<![CDATA[{formattedCode}]]>";
            }

            return this;
        }

        public TcPou FormatImplementation()
        {
            uint indents = 0;
            XmlNodeList nodes = doc.SelectNodes(".//Implementation/ST");
            foreach (XmlNode node in nodes)
            {
                string formattedCode =
                    new ImplementationCode(node.InnerText, Indentation, LineEnding)
                    .Tokenize()
                    .Format(ref indents);
                string firstLine;
                if (
                    formattedCode.StartsWith("//")
                    || formattedCode.StartsWith(LineEnding)
                )
                {
                    firstLine = "";
                }
                else
                {
                    firstLine = LineEnding;
                }

                node.InnerXml = $"<![CDATA[{firstLine}{formattedCode}]]>";
            }

            return this;
        }

        /// <summary>
        /// Overwrites the currently saved TcPOU file on disk.
        /// </summary>
        public void Save()
        {
            using (var w = XmlWriter.Create(_path, new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = LineEnding,
            }))
            {
                doc.Save(w);
            }
        }

        /// <summary>
        /// Return the line ending from the TcPOU file.
        /// </summary>
        private string LineEnding { get; set; }

        /// <summary>
        /// Return the indentation type of the TcPOU file. Either tabs or four spaces.
        /// </summary>
        private string Indentation { get; set; }
    }
}
