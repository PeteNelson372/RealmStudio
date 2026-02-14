using SkiaSharp;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudioX
{
    public sealed class DrawnLine : DrawnMapComponent, IXmlSerializable
    {
        private List<SKPoint> _points = [];
        private SKColor _color = SKColors.Black;
        private int _brushSize = 2;

        public DrawnLine()
        {
            // Default constructor
        }

        public List<SKPoint> Points
        {
            get => _points;
            set
            {
                _points = value ?? throw new ArgumentNullException(nameof(value), "Points cannot be null.");
            }
        }

        public SKColor Color
        {
            get => _color;
            set
            {
                _color = value;
            }
        }

        public int BrushSize
        {
            get => _brushSize;
            set
            {
                _brushSize = value;
            }
        }

        public override void Render(SKCanvas canvas)
        {
            SKPath path = new();
            path.MoveTo(_points[0]);

            for (int i = 1; i < _points.Count; i++)
            {
                path.LineTo(_points[i]);
            }

            using SKPaint paint = new()
            {
                Style = SKPaintStyle.Stroke,
                Color = Color,
                StrokeWidth = BrushSize,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round
            };

            canvas.DrawPath(path, paint);
            path.GetBounds(out SKRect bounds);

            Bounds = bounds;

            base.Render(canvas);

            for (int i = 0; i < _points.Count - 1; i++)
            {
                SKPoint start = _points[i];
                SKPoint end = _points[i + 1];

                canvas.DrawLine(start, end, paint);
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();

            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            XDocument lineDoc = XDocument.Parse(content);

            if (lineDoc.Root == null)
            {
                throw new InvalidDataException("The line XML data is missing the root element.");
            }

            XAttribute? xAttr = lineDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = lineDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? wAttr = lineDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = (int)float.Parse(wAttr.Value);
            }

            XAttribute? hAttr = lineDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = (int)float.Parse(hAttr.Value);
            }


            XElement? colorElement = lineDoc.Root.Element(ns + "Color");
            if (colorElement != null)
            {
                _color = SKColor.Parse(colorElement.Value);
            }

            XElement? brushSizeElement = lineDoc.Root.Element(ns + "BrushSize");
            if (brushSizeElement != null)
            {
                _brushSize = (int)float.Parse(brushSizeElement.Value);
            }

            XElement? pointsElement = lineDoc.Root.Element(ns + "Points");
            if (pointsElement != null)
            {
                List<SKPoint> points = [];
                foreach (XElement pointElement in pointsElement.Elements(ns + "Point"))
                {
                    XElement? xElement = pointElement.Element(ns + "X");
                    XElement? yElement = pointElement.Element(ns + "Y");
                    if (xElement != null && yElement != null &&
                        float.TryParse(xElement.Value, out float x) &&
                        float.TryParse(yElement.Value, out float y))
                    {
                        points.Add(new SKPoint(x, y));
                    }
                }

                Points = points;

                int minX = (int)Points.Min(p => p.X);
                int minY = (int)Points.Min(p => p.Y);
                int maxX = (int)Points.Max(p => p.X);
                int maxY = (int)Points.Max(p => p.Y);

                X = minX;
                Y = minY;
                Width = maxX - minX;
                Height = maxY - minY;

                Bounds = new SKRect(minX, minY, maxX, maxY);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            writer.WriteElementString("Color", Color.ToString());
            writer.WriteElementString("BrushSize", BrushSize.ToString());

            writer.WriteStartElement("Points");
            for (int i = 0; i < Points.Count; i++)
            {
                writer.WriteStartElement("Point");
                writer.WriteAttributeString("Index", i.ToString());
                writer.WriteElementString("X", Points[i].X.ToString());
                writer.WriteElementString("Y", Points[i].Y.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Points
        }
    }
}
