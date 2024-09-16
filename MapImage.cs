using SkiaSharp;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    internal class MapImage : MapComponent, IXmlSerializable
    {
        public SKBitmap? MapImageBitmap { get; set; }

        public MapImage() { }

        public override void Render(SKCanvas canvas)
        {
            if (MapImageBitmap != null)
            {
                canvas.DrawBitmap(MapImageBitmap, 0, 0);
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8601 // Possible null reference assignment.

            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();
            XDocument mapBitmapDoc = XDocument.Parse(content);

            string? base64String = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "Bitmap").Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(base64String))
            {
                // Convert Base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Create an image from the byte array
                using (MemoryStream ms = new(imageBytes))
                {
                    MapImageBitmap = SKBitmap.Decode(ms);

                    Width = MapImageBitmap.Width;
                    Height = MapImageBitmap.Height;
                }
            }

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            if (MapImageBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);
                MapImageBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] bitmapData = ms.ToArray();
                writer.WriteStartElement("Bitmap");
                writer.WriteBase64(bitmapData, 0, bitmapData.Length);
                writer.WriteEndElement();
            }
        }
    }
}
