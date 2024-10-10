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
    public class MapRiverPoint : IXmlSerializable
    {
        public Guid PointGuid { get; set; } = Guid.NewGuid();
        public SKPoint RiverPoint { get; set; }

        [XmlIgnore]
        public bool IsSelected { get; set; } = false;

        public MapRiverPoint() { }

        public MapRiverPoint(SKPoint point)
        {
            RiverPoint = point;
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();
            XDocument mapRiverPointDoc = XDocument.Parse(content);

            IEnumerable<XElement?> guidElemEnum = mapRiverPointDoc.Descendants().Select(x => x.Element(ns + "PointGuid"));
            if (guidElemEnum.First() != null)
            {
                string? mapGuid = mapRiverPointDoc.Descendants().Select(x => x.Element(ns + "PointGuid").Value).FirstOrDefault();
                PointGuid = Guid.Parse(mapGuid);
            }

            IEnumerable<XElement?> xyElemEnum = mapRiverPointDoc.Descendants().Select(x => x.Element(ns + "RiverPoint"));
            if (xyElemEnum.First() != null)
            {
                List<XElement> elemList = xyElemEnum.Descendants().ToList();

                if (elemList != null)
                {
                    float x = 0;
                    float y = 0;

                    foreach (XElement elem in elemList)
                    {
                        if (elem.Name.LocalName.ToString() == "X")
                        {
                            x = float.Parse(elem.Value);
                        }

                        if (elem.Name.LocalName.ToString() == "Y")
                        {
                            y = float.Parse(elem.Value);
                            RiverPoint = new SKPoint(x, y);
                        }
                    }
                }
            }

#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PointGuid");
            writer.WriteString(PointGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RiverPoint");

            writer.WriteStartElement("X");
            writer.WriteValue(RiverPoint.X.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Y");
            writer.WriteValue(RiverPoint.Y.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
