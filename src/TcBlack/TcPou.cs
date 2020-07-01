using System.Xml;

namespace TcBlack
{
    /// <summary>
    /// Load, format and save a TcPOU file.
    /// </summary>
    public class TcPou
    {
        private XmlDocument doc;
        private string tcPouPath;
        private string text;

        /// <summary>
        /// Loads a TcPOU file.
        /// </summary>
        /// <param name="path">Path to the TcPOU file.</param>
        public TcPou(string path)
        {
            tcPouPath = path;
            doc = new XmlDocument();
            doc.Load(path);

            text = doc.InnerXml;
            LineEnding = text.Contains("\r\n") ? "\r\n" : "\n";
            Indentation = text.Contains("\t") ? "\t" : "    ";
        }

        /// <summary>
        /// Loads a TcPOU file.
        /// </summary>
        /// <param name="path">Path to the TcPOU file.</param>
        /// <param name="indentation">
        /// Which indentation to use in the formatted file.
        /// </param>
        public TcPou(string path, string indentation) : this(path)
        {
            Indentation = indentation;
        }

        /// <summary>
        /// Loads a TcPOU file.
        /// </summary>
        /// <param name="path">Path to the TcPOU file.</param>
        /// <param name="windowsLineEnding">If true use '\r\n' else uses '\n'.</param>
        public TcPou(string path, bool windowsLineEnding) : this(path)
        {
            LineEnding = windowsLineEnding ? "\r\n" : "\n";
        }

        /// <summary>
        /// Loads a TcPOU file.
        /// </summary>
        /// <param name="path">Path to the TcPOU file.</param>
        /// <param name="indentation">
        /// Which indentation to use in the formatted file.
        /// </param>
        /// <param name="windowsLineEnding">If true use '\r\n' else uses '\n'.</param>
        public TcPou(string path, string indentation, bool windowsLineEnding) 
            : this(path, windowsLineEnding)
        {
            Indentation = indentation;
        }

        /// <summary>
        /// Format the TwinCAT TcPOU file.
        /// </summary>
        /// <returns>The formatted TcPOU object.</returns>
        public TcPou Format()
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

        /// <summary>
        /// Overwrites the currently saved TcPOU file on disk.
        /// </summary>
        public void Save()
        {
            using (var w = XmlWriter.Create(tcPouPath, new XmlWriterSettings
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
