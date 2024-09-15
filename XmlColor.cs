using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public struct XmlColor(Color source) : IXmlSerializable
    {
        private Color colorValue = source;

        public static implicit operator Color(XmlColor c)
        {
            return c.colorValue;
        }

        public static implicit operator XmlColor(Color c)
        {
            return new XmlColor(c);
        }

        public readonly XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XElement el = (XElement)XNode.ReadFrom(reader);

            string colorAsString = el.Value;
            colorValue = ColorTranslator.FromHtml(colorAsString);
        }

        public readonly void WriteXml(XmlWriter writer)
        {
            string colorAsString = ColorTranslator.ToHtml(colorValue);
            writer.WriteString(colorAsString);
        }

        [XmlText]
        public string Default
        {
            get { return ColorTranslator.ToHtml(colorValue); }
            set { colorValue = ColorTranslator.FromHtml(value); }
        }
    }
}
