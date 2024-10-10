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
using SkiaSharp.Views.Desktop;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapWindrose : MapComponent, IXmlSerializable
    {
        public Guid WindroseGuid { get; set; } = Guid.NewGuid();
        public Color WindroseColor { get; set; } = ColorTranslator.FromHtml("#7F3D3728");
        public int DirectionCount { get; set; } = 16;
        public int LineWidth { get; set; } = 2;
        public int InnerRadius { get; set; } = 0;
        public int OuterRadius { get; set; } = 1000;
        public int InnerCircles { get; set; } = 0;
        public bool FadeOut { get; set; } = false;

        public SKPaint? WindrosePaint { get; set; } = null;

        public override void Render(SKCanvas canvas)
        {
            if (WindrosePaint != null)
            {
                // draw circles, if any
                if (InnerRadius > 0)
                {
                    switch (InnerCircles)
                    {
                        case 1:
                            canvas.DrawCircle(X, Y, InnerRadius, WindrosePaint);
                            break;
                        case 2:
                            canvas.DrawCircle(X, Y, InnerRadius / 2, WindrosePaint);
                            canvas.DrawCircle(X, Y, InnerRadius, WindrosePaint);
                            break;
                    }

                    DrawWindroseLines(canvas, true);
                }
                else
                {
                    DrawWindroseLines(canvas, false);
                }
            }

        }

        private void DrawWindroseLines(SKCanvas canvas, bool fromCircle)
        {
            float circleStep = 360.0F / DirectionCount;

            for (int i = 0; i < DirectionCount; i++)
            {
                SKPoint sp;

                if (fromCircle)
                {
                    // get the starting point on the perimeter of the inner circle
                    sp = DrawingMethods.PointOnCircle(InnerRadius, i * circleStep, new SKPoint(X, Y));
                }
                else
                {
                    // get the starting point at the cursor point
                    sp = new SKPoint(X, Y);
                }

                // get the ending point at the outer radius
                SKPoint ep = DrawingMethods.PointOnCircle(OuterRadius, i * circleStep, new SKPoint(X, Y));

                if (FadeOut)
                {
                    DrawFadedLine(canvas, sp, ep);
                }
                else
                {
                    canvas.DrawLine(sp, ep, WindrosePaint);
                }
            }
        }

        private void DrawFadedLine(SKCanvas canvas, SKPoint sp, SKPoint ep)
        {
            if (WindrosePaint != null)
            {
                SKPoint[] linePoints = DrawingMethods.GetPoints(254, sp, ep);
                int segmentOpacity = WindroseColor.A;
                Color argbColor = WindroseColor;

                for (int j = 0; j < linePoints.Length - 1; j++)
                {
                    // draw line between points in linePoints, decreasing opacity

                    argbColor = Color.FromArgb(segmentOpacity, WindroseColor);
                    WindrosePaint.Color = argbColor.ToSKColor();

                    canvas.DrawLine(linePoints[j], linePoints[j + 1], WindrosePaint);

                    segmentOpacity--;
                    segmentOpacity = Math.Max(segmentOpacity, 0);
                }
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
            XDocument mapRoseDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapRoseDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapRoseDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapRoseDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapRoseDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> guidElemEnum = mapRoseDoc.Descendants().Select(x => x.Element(ns + "WindroseGuid"));
            if (guidElemEnum.First() != null)
            {
                string? windroseGuid = mapRoseDoc.Descendants().Select(x => x.Element(ns + "WindroseGuid").Value).FirstOrDefault();
                WindroseGuid = Guid.Parse(windroseGuid);
            }

            IEnumerable<XElement> windroseColorElem = mapRoseDoc.Descendants(ns + "WindroseColor");
            if (windroseColorElem != null && windroseColorElem.Count() > 0 && windroseColorElem.First() != null)
            {
                string? gridColor = mapRoseDoc.Descendants().Select(x => x.Element(ns + "WindroseColor").Value).FirstOrDefault();

                int argbValue = 0;
                if (int.TryParse(gridColor, out int n))
                {
                    if (n > 0)
                    {
                        argbValue = n;
                    }
                    else
                    {
                        argbValue = ColorTranslator.FromHtml("#7F3D3728").ToArgb();
                    }
                }

                WindroseColor = Color.FromArgb(argbValue);
            }
            else
            {
                WindroseColor = ColorTranslator.FromHtml("#7F3D3728");
            }

            IEnumerable<XElement?> directionCountElem = mapRoseDoc.Descendants().Select(x => x.Element(ns + "DirectionCount"));
            if (directionCountElem.First() != null)
            {
                string? directionCount = mapRoseDoc.Descendants().Select(x => x.Element(ns + "DirectionCount").Value).FirstOrDefault();
                DirectionCount = int.Parse(directionCount);
            }

            IEnumerable<XElement?> lineWidthElem = mapRoseDoc.Descendants().Select(x => x.Element(ns + "LineWidth"));
            if (lineWidthElem.First() != null)
            {
                string? lineWidth = mapRoseDoc.Descendants().Select(x => x.Element(ns + "LineWidth").Value).FirstOrDefault();
                LineWidth = int.Parse(lineWidth);
            }

            IEnumerable<XElement?> innerRadiusElem = mapRoseDoc.Descendants().Select(x => x.Element(ns + "InnerRadius"));
            if (innerRadiusElem.First() != null)
            {
                string? innerRadius = mapRoseDoc.Descendants().Select(x => x.Element(ns + "InnerRadius").Value).FirstOrDefault();
                InnerRadius = int.Parse(innerRadius);
            }

            IEnumerable<XElement?> outerRadiusElem = mapRoseDoc.Descendants().Select(x => x.Element(ns + "OuterRadius"));
            if (outerRadiusElem.First() != null)
            {
                string? outerRadius = mapRoseDoc.Descendants().Select(x => x.Element(ns + "OuterRadius").Value).FirstOrDefault();
                OuterRadius = int.Parse(outerRadius);
            }

            IEnumerable<XElement?> innerCirclesElem = mapRoseDoc.Descendants().Select(x => x.Element(ns + "InnerCircles"));
            if (innerCirclesElem.First() != null)
            {
                string? innerCircles = mapRoseDoc.Descendants().Select(x => x.Element(ns + "InnerCircles").Value).FirstOrDefault();
                InnerCircles = int.Parse(innerCircles);
            }

            IEnumerable<XElement?> fadeOutElem = mapRoseDoc.Descendants().Select(x => x.Element(ns + "FadeOut"));
            if (fadeOutElem.First() != null)
            {
                string? fadeOut = mapRoseDoc.Descendants().Select(x => x.Element(ns + "FadeOut").Value).FirstOrDefault();
                FadeOut = bool.Parse(fadeOut);
            }

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            writer.WriteStartElement("WindroseGuid");
            writer.WriteString(WindroseGuid.ToString());
            writer.WriteEndElement();

            int windroseColor = WindroseColor.ToArgb();
            writer.WriteStartElement("WindroseColor");
            writer.WriteValue(windroseColor);
            writer.WriteEndElement();

            writer.WriteStartElement("DirectionCount");
            writer.WriteValue(DirectionCount);
            writer.WriteEndElement();

            writer.WriteStartElement("LineWidth");
            writer.WriteValue(LineWidth);
            writer.WriteEndElement();

            writer.WriteStartElement("InnerRadius");
            writer.WriteValue(InnerRadius);
            writer.WriteEndElement();

            writer.WriteStartElement("OuterRadius");
            writer.WriteValue(OuterRadius);
            writer.WriteEndElement();

            writer.WriteStartElement("InnerCircles");
            writer.WriteValue(InnerCircles);
            writer.WriteEndElement();

            writer.WriteStartElement("FadeOut");
            writer.WriteValue(FadeOut);
            writer.WriteEndElement();
        }
    }
}
