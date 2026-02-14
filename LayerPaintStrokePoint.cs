/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
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

namespace RealmStudioX
{
    public class LayerPaintStrokePoint : MapComponent, IXmlSerializable
    {
        [XmlElement]
        public Guid Id { get; set; } = Guid.NewGuid();
        [XmlElement]
        public SKPoint StrokeLocation { get; set; } = SKPoint.Empty;
        [XmlElement]
        public int StrokeRadius { get; set; } = 0;

        public LayerPaintStrokePoint() { }

        public LayerPaintStrokePoint(SKPoint strokeLocation, int strokeRadius)
        {
            StrokeLocation = strokeLocation;
            StrokeRadius = strokeRadius;

            X = (int)StrokeLocation.X;
            Y = (int)StrokeLocation.Y;
            Width = StrokeRadius * 2;
            Height = StrokeRadius * 2;
        }

        public override void Render(SKCanvas canvas)
        {
            // layer paint strokes are rendered as a part of a path when the layer is rendered
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            string content = reader.ReadOuterXml();
            XDocument strokePointDoc = XDocument.Parse(content);

            XAttribute? idAttr = strokePointDoc.Root?.Attribute("Id");
            if (idAttr != null)
            {
                Id = Guid.Parse(idAttr.Value);
            }

            XAttribute? xAttr = strokePointDoc.Root?.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = strokePointDoc.Root?.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? radiusAttr = strokePointDoc.Root?.Attribute("Radius");
            if (radiusAttr != null)
            {
                StrokeRadius = int.Parse(radiusAttr.Value);
            }

            StrokeLocation = new SKPoint(X, Y);
            Width = StrokeRadius * 2;
            Height = StrokeRadius * 2;

        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("X", StrokeLocation.X.ToString());
            writer.WriteAttributeString("Y", StrokeLocation.Y.ToString());
            writer.WriteAttributeString("Radius", StrokeRadius.ToString());
            writer.WriteAttributeString("Height", Height.ToString());
        }
    }
}
