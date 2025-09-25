/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public sealed class DrawingErase : DrawnMapComponent, IXmlSerializable
    {
        private int _brushSize = 2;
        private List<SKPoint> _points = [];

        public List<SKPoint> Points
        {
            get => _points;
            set
            {
                _points = value ?? throw new ArgumentNullException(nameof(value), "Points cannot be null.");
            }
        }

        public int BrushSize
        {
            get => _brushSize;
            set => _brushSize = value;
        }

        public override void Render(SKCanvas canvas)
        {
            using SKPaint paint = new()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Transparent,
                StrokeWidth = BrushSize / 2,
                IsAntialias = true,
                BlendMode = SKBlendMode.Clear
            };

            foreach (SKPoint erasePoint in Points)
            {
                canvas.DrawCircle(erasePoint, BrushSize / 2, paint);
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

            XDocument eraseDoc = XDocument.Parse(content);

            XElement? root = eraseDoc.Element(ns + "DrawingErase");

            if (root != null)
            {
                XElement? brushSizeElement = root.Element(ns + "BrushSize");
                if (brushSizeElement != null && int.TryParse(brushSizeElement.Value, out int brushSize))
                {
                    BrushSize = brushSize;
                }
                XElement? pointsElement = root.Element(ns + "Points");
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
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("BrushSize", BrushSize.ToString());

            writer.WriteStartElement("Points");
            foreach (SKPoint point in Points)
            {
                writer.WriteStartElement("Point");
                writer.WriteElementString("X", point.X.ToString());
                writer.WriteElementString("Y", point.Y.ToString());
                writer.WriteEndElement(); // Point
            }
            writer.WriteEndElement(); // Points
        }
    }
}
