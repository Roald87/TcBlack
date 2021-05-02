using System.Xml;
using TcBlackCore;

namespace TcBlackCLI
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
            Globals.lineEnding = text.Contains("\r\n") ? "\r\n" : "\n";
            Globals.indentation = text.Contains("\t") ? "\t" : "    ";
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
            Globals.indentation = indentation;
        }

        /// <summary>
        /// Loads a TcPOU file.
        /// </summary>
        /// <param name="path">Path to the TcPOU file.</param>
        /// <param name="windowsLineEnding">If true use '\r\n' else uses '\n'.</param>
        public TcPou(string path, bool windowsLineEnding) : this(path)
        {
            Globals.lineEnding = windowsLineEnding ? "\r\n" : "\n";
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
            Globals.indentation = indentation;
        }

        /// <summary>
        /// Format the TwinCAT TcPOU file.
        /// </summary>
        /// <returns>The formatted TcPOU object.</returns>
        public TcPou Format()
        {
            int indents = 0;
            XmlNodeList nodes = doc.SelectNodes(".//Declaration");
            foreach (XmlNode node in nodes)
            {
                string formattedCode = 
                    new CompositeCode(node.InnerText).Tokenize().Format(ref indents);
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
                NewLineChars = Globals.lineEnding,
            }))
            {
                doc.Save(w);
            }
        }
    }
}
