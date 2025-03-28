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

namespace RealmStudio
{
    public class MapRegionPoint : IXmlSerializable
    {
        public Guid PointGuid { get; set; } = Guid.NewGuid();
        public SKPoint RegionPoint { get; set; }

        public bool IsSelected { get; set; }

        public MapRegionPoint() { }

        public MapRegionPoint(SKPoint point)
        {
            RegionPoint = point;
        }

        public void Render(SKCanvas canvas)
        {
            if (IsSelected)
            {
                canvas.DrawCircle(RegionPoint, RegionManager.POINT_CIRCLE_RADIUS, PaintObjects.RegionPointSelectedFillPaint);
            }
            else
            {
                canvas.DrawCircle(RegionPoint, RegionManager.POINT_CIRCLE_RADIUS, PaintObjects.RegionPointFillPaint);
            }

            canvas.DrawCircle(RegionPoint, RegionManager.POINT_CIRCLE_RADIUS, PaintObjects.RegionPointOutlinePaint);
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            string content = reader.ReadOuterXml();
            XDocument mapRegionPointDoc = XDocument.Parse(content);

            XAttribute? idAttr = mapRegionPointDoc.Root?.Attribute("PointGuid");
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
            XAttribute? xAttr = mapRegionPointDoc.Root?.Attribute("X");
            if (xAttr != null)
            {
                x = float.Parse(xAttr.Value);
            }

            float y = 0;
            XAttribute? yAttr = mapRegionPointDoc.Root?.Attribute("Y");
            if (yAttr != null)
            {
                y = float.Parse(yAttr.Value);
            }

            RegionPoint = new SKPoint(x, y);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("PointGuid", PointGuid.ToString());
            writer.WriteAttributeString("X", RegionPoint.X.ToString());
            writer.WriteAttributeString("Y", RegionPoint.Y.ToString());
        }
    }
}
