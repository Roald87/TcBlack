using System.Xml;

namespace TcBlack
{
    public struct TcPou
    {
        public TcPou(
            string declaration
        )
        {
            Declaration = declaration;
        }

        public string Declaration { get; }
    }

    public class TcPouReader
    {
        private XmlDocument _doc;

        public TcPouReader(string path)
        {
            _doc = new XmlDocument();
            _doc.Load(path);
        }

        public TcPou Read()
        {
            string declarations = _doc
                .SelectSingleNode("/TcPlcObject/POU/Declaration")
                .InnerText;

            TcPou tcPou = new TcPou(declaration: declarations);

            return tcPou;
        }
    }
}
