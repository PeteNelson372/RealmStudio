/**************************************************************************************************************************
* Copyright 2026, Peter R. Nelson
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

/** ***************************************************************
 * LinePoint represents a point in a line segment with serialization capabilities.
 * It is the same as MapPathPoint but both are retained for backward compatibility.
 ****************************************************************/

namespace RealmStudio
{
    public class LinePoint : IXmlSerializable
    {
        public Guid PointGuid { get; set; } = Guid.NewGuid();
        public SKPoint LineSegmentPoint { get; set; }

        [XmlIgnore]
        public bool IsSelected { get; set; } = false;

        [XmlIgnore]
        public bool IsControlPoint { get; set; } = false;

        public LinePoint() { }

        public LinePoint(SKPoint point)
        {
            LineSegmentPoint = point;
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            string content = reader.ReadOuterXml();
            XDocument mapPathPointDoc = XDocument.Parse(content);

            XAttribute? idAttr = mapPathPointDoc.Root?.Attribute("PointGuid");
            if (idAttr != null)
            {
                PointGuid = Guid.Parse(idAttr.Value);
            }
            else
            {
                PointGuid = Guid.NewGuid();
            }

            // TODO: use tryparse for reliability
            float x = 0;
            XAttribute? xAttr = mapPathPointDoc.Root?.Attribute("X");
            if (xAttr != null)
            {
                x = float.Parse(xAttr.Value);
            }

            float y = 0;
            XAttribute? yAttr = mapPathPointDoc.Root?.Attribute("Y");
            if (yAttr != null)
            {
                y = float.Parse(yAttr.Value);
            }

            LineSegmentPoint = new SKPoint(x, y);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("PointGuid", PointGuid.ToString());
            writer.WriteAttributeString("X", LineSegmentPoint.X.ToString());
            writer.WriteAttributeString("Y", LineSegmentPoint.Y.ToString());
        }
    }
}
