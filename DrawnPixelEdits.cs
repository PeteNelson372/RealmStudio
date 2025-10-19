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
using SkiaSharp.Views.Desktop;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public sealed class DrawnPixelEdits : DrawnMapComponent, IXmlSerializable
    {
        private List<Tuple<SKPoint, Color, Color>> _mapPixelEdits = [];
        public List<Tuple<SKPoint, Color, Color>> MapPixelEdits
        {
            get => _mapPixelEdits;
            set => _mapPixelEdits = value;
        }

        public override void Render(SKCanvas canvas)
        {
            using SKImage? bitmap = canvas.Surface?.Snapshot();

            if (bitmap != null)
            {
                using SKBitmap skBitmap = SKBitmap.FromImage(bitmap);
                foreach (var edit in _mapPixelEdits)
                {
                    SKPoint location = edit.Item1;
                    Color targetColor = edit.Item3;

                    skBitmap.SetPixel((int)location.X, (int)location.Y, targetColor.ToSKColor());
                }
                canvas.DrawBitmap(skBitmap.Copy(), 0, 0);
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

            XDocument pixelEditDoc = XDocument.Parse(content);

            if (pixelEditDoc.Root == null)
            {
                throw new InvalidDataException("The pixel edit XML data is missing the root element.");
            }

            XAttribute? xAttr = pixelEditDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = pixelEditDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? wAttr = pixelEditDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = (int)float.Parse(wAttr.Value);
            }

            XAttribute? hAttr = pixelEditDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = (int)float.Parse(hAttr.Value);
            }

            XElement? pixelEditsElement = pixelEditDoc.Root.Element(ns + "PixelEdits");
            if (pixelEditsElement != null)
            {
                IEnumerable<XElement> pixelEditElements = pixelEditsElement.Elements(ns + "PixelEdit");
                foreach (XElement pixelEditElement in pixelEditElements)
                {
                    XAttribute? pxAttr = pixelEditElement.Attribute("X");
                    XAttribute? pyAttr = pixelEditElement.Attribute("Y");
                    XAttribute? originalColorAttr = pixelEditElement.Attribute("OriginalColor");
                    XAttribute? targetColorAttr = pixelEditElement.Attribute("TargetColor");
                    if (pxAttr != null && pyAttr != null && originalColorAttr != null && targetColorAttr != null)
                    {
                        SKPoint location = new((int)float.Parse(pxAttr.Value), (int)float.Parse(pyAttr.Value));
                        Color originalColor = Color.FromArgb(int.Parse(originalColorAttr.Value, System.Globalization.NumberStyles.HexNumber));
                        Color targetColor = Color.FromArgb(int.Parse(targetColorAttr.Value, System.Globalization.NumberStyles.HexNumber));
                        _mapPixelEdits.Add(new Tuple<SKPoint, Color, Color>(location, originalColor, targetColor));
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            writer.WriteStartElement("PixelEdits");
            foreach (var edit in _mapPixelEdits)
            {
                writer.WriteStartElement("PixelEdit");
                writer.WriteAttributeString("X", edit.Item1.X.ToString());
                writer.WriteAttributeString("Y", edit.Item1.Y.ToString());
                writer.WriteAttributeString("OriginalColor", edit.Item2.ToArgb().ToString("X8"));
                writer.WriteAttributeString("TargetColor", edit.Item3.ToArgb().ToString("X8"));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
