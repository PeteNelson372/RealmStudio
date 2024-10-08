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
* contact@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class MapScale : MapComponent, IXmlSerializable
    {
        public Guid ScaleGuid { get; set; } = Guid.NewGuid();

        // X, Y inherited from MapComponent are position of the map scale
        // Width, Height inherited from MapComponent are total width and height of the map scale

        public int ScaleSegmentCount { get; set; } = 5;  // how many segments in the scale
        public int ScaleLineWidth { get; set; } = 3;  // width of the outline around the scale
        public Color ScaleColor1 { get; set; } = Color.Black;  // odd numberd segment color
        public Color ScaleColor2 { get; set; } = Color.White;  // even numbered segment color
        public Color ScaleColor3 { get; set; } = Color.Black;  // line color of scale outline
        public float ScaleDistance { get; set; } = 100.0F;  // distance of each segment
        public string ScaleDistanceUnit { get; set; } = string.Empty;  // feet, meters, miles, kilometers, etc.
        public ScaleNumbersDisplayEnum ScaleNumbersDisplayType { get; set; } = ScaleNumbersDisplayEnum.All;  // where to display the segment labels
        public Font ScaleFont { get; set; } = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0); // scale segment label font, color, outline
        public Color ScaleFontColor { get; set; } = Color.White;
        public int ScaleOutlineWidth { get; set; } = 2;
        public Color ScaleOutlineColor { get; set; } = Color.Black;

        public bool IsSelected { get; set; } = false;

        private SKPaint SegmentOutlinePaint = new();
        private SKPaint EvenSegmentPaint = new();
        private SKPaint OddSegmentPaint = new();
        private SKPaint LabelPaint = new();
        private SKPaint OutlinePaint = new();

        public MapScale()
        {
            ConstructPaintObjects();
        }

        private void ConstructPaintObjects()
        {
            SegmentOutlinePaint = new()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = ScaleLineWidth,
                Color = ScaleColor3.ToSKColor()
            };

            EvenSegmentPaint = new()
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = Height - ScaleLineWidth,
                Color = ScaleColor1.ToSKColor()
            };

            OddSegmentPaint = new()
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = Height - ScaleLineWidth,
                Color = ScaleColor2.ToSKColor()
            };

            LabelPaint = new()
            {
                Color = Extensions.ToSKColor(ScaleFontColor),
                TextSize = (ScaleFont.Size * 4.0F) / 3.0F,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };

            SKFontStyle fs = SKFontStyle.Normal;

            if (ScaleFont.Bold && ScaleFont.Italic)
            {
                fs = SKFontStyle.BoldItalic;
            }
            else if (ScaleFont.Bold)
            {
                fs = SKFontStyle.Bold;
            }
            else if (ScaleFont.Italic)
            {
                fs = SKFontStyle.Italic;
            }

            LabelPaint.Typeface = SKFontManager.Default.MatchFamily(ScaleFont.Name, fs);

            OutlinePaint = new()
            {
                Color = ScaleOutlineColor.ToSKColor(),
                TextSize = (ScaleFont.Size * 4.0F) / 3.0F,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDilate(ScaleOutlineWidth, ScaleOutlineWidth),
            };
        }

        public override void Render(SKCanvas canvas)
        {
            float segmentWidth = Width / ScaleSegmentCount;

            ConstructPaintObjects();

            // draw the scale outline
            SKRect outlineRect = new(X, Y, X + Width, Y + Height);

            canvas.DrawRect(outlineRect, SegmentOutlinePaint);

            // draw the segments and labels
            int segmentX = (int)X + (ScaleLineWidth / 2);
            int segmentY = (int)((int)Y + (Height / 2));

            for (int i = 0; i < ScaleSegmentCount; i++)
            {
                SKPoint sp = new(segmentX, segmentY);
                SKPoint ep = new(segmentX + segmentWidth, segmentY);

                if (int.IsEvenInteger(i))
                {
                    canvas.DrawLine(sp, ep, EvenSegmentPaint);
                }
                else
                {
                    canvas.DrawLine(sp, ep, OddSegmentPaint);
                }

                segmentX = (int)(segmentX + segmentWidth);
            }

            // draw the distance labels
            int labelX = X + (ScaleLineWidth / 2);
            int labelY = Y;

            float distance = 0;

            for (int i = 0; i <= ScaleSegmentCount; i++)
            {
                string distanceText = string.Format("{0}", (int)distance);
                SKRect bounds = new();

                LabelPaint.MeasureText(distanceText, ref bounds);


                SKPoint labelPoint = new(labelX, labelY - bounds.Height);

                switch (ScaleNumbersDisplayType)
                {
                    case ScaleNumbersDisplayEnum.Ends:
                        {
                            if (i == 0 || i == ScaleSegmentCount)
                            {
                                canvas.DrawText(distanceText, labelPoint, OutlinePaint);
                                canvas.DrawText(distanceText, labelPoint, LabelPaint);
                            }
                        }
                        break;
                    case ScaleNumbersDisplayEnum.EveryOther:
                        {
                            if (int.IsEvenInteger(i))
                            {
                                canvas.DrawText(distanceText, labelPoint, OutlinePaint);
                                canvas.DrawText(distanceText, labelPoint, LabelPaint);
                            }
                        }
                        break;
                    case ScaleNumbersDisplayEnum.All:
                        {
                            canvas.DrawText(distanceText, labelPoint, OutlinePaint);
                            canvas.DrawText(distanceText, labelPoint, LabelPaint);
                        }
                        break;
                }

                distance += ScaleDistance;
                labelX = (int)(labelX + segmentWidth);
            }

            if (!string.IsNullOrEmpty(ScaleDistanceUnit))
            {
                // draw the scale unit label
                int unitLabelX = (int)((int)X + (Width / 2));
                int unitLabelY = (int)((int)Y + Height);

                SKRect bounds = new();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                LabelPaint.MeasureText(ScaleDistanceUnit, ref bounds);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                unitLabelY = (int)(unitLabelY + bounds.Height * 2);

                SKPoint unitLabelPoint = new SKPoint(unitLabelX, unitLabelY);
                canvas.DrawText(ScaleDistanceUnit, unitLabelPoint, OutlinePaint);
                canvas.DrawText(ScaleDistanceUnit, unitLabelPoint, LabelPaint);
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
            XDocument mapScaleDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapScaleDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapScaleDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapScaleDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapScaleDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> guidElemEnum = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleGuid"));
            if (guidElemEnum.First() != null)
            {
                string? scaleGuid = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleGuid").Value).FirstOrDefault();
                ScaleGuid = Guid.Parse(scaleGuid);
            }

            IEnumerable<XElement?> scaleSegmentCountElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleSegmentCount"));
            if (scaleSegmentCountElem.First() != null)
            {
                string? scaleSegmentCount = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleSegmentCount").Value).FirstOrDefault();
                ScaleSegmentCount = int.Parse(scaleSegmentCount);
            }

            IEnumerable<XElement?> scaleLineWidthElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleLineWidth"));
            if (scaleLineWidthElem.First() != null)
            {
                string? scaleLineWidth = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleLineWidth").Value).FirstOrDefault();
                ScaleLineWidth = int.Parse(scaleLineWidth);
            }

            IEnumerable<XElement> scaleColor1Elem = mapScaleDoc.Descendants(ns + "ScaleColor1");
            if (scaleColor1Elem.First() != null)
            {
                string? scaleColor1 = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleColor1").Value).FirstOrDefault();
                ScaleColor1 = ColorTranslator.FromHtml(scaleColor1);
            }

            IEnumerable<XElement> scaleColor2Elem = mapScaleDoc.Descendants(ns + "ScaleColor2");
            if (scaleColor2Elem.First() != null)
            {
                string? scaleColor2 = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleColor2").Value).FirstOrDefault();
                ScaleColor2 = ColorTranslator.FromHtml(scaleColor2);
            }

            IEnumerable<XElement> scaleColor3Elem = mapScaleDoc.Descendants(ns + "ScaleColor3");
            if (scaleColor3Elem.First() != null)
            {
                string? scaleColor3 = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleColor3").Value).FirstOrDefault();
                ScaleColor3 = ColorTranslator.FromHtml(scaleColor3);
            }

            IEnumerable<XElement?> scaleDistanceElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleDistance"));
            if (scaleDistanceElem.First() != null)
            {
                string? scaleDistance = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleDistance").Value).FirstOrDefault();
                ScaleDistance = int.Parse(scaleDistance);
            }

            IEnumerable<XElement?> scaleDistanceUnitElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleDistanceUnit"));
            if (scaleDistanceUnitElem != null && scaleDistanceUnitElem.First() != null)
            {
                ScaleDistanceUnit = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleDistanceUnit").Value).FirstOrDefault();
            }

            IEnumerable<XElement?> typeElemEnum = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleNumbersDisplayType"));
            if (typeElemEnum.First() != null)
            {
                string? scaleNumbersDisplayType = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleNumbersDisplayType").Value).FirstOrDefault();
                ScaleNumbersDisplayType = Enum.Parse<ScaleNumbersDisplayEnum>(scaleNumbersDisplayType);
            }

            IEnumerable<XElement> scaleFontColorElem = mapScaleDoc.Descendants(ns + "ScaleFontColor");
            if (scaleFontColorElem.First() != null)
            {
                string? scaleFontColor = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleFontColor").Value).FirstOrDefault();
                ScaleFontColor = ColorTranslator.FromHtml(scaleFontColor);
            }

            IEnumerable<XElement?> scaleOutlineWidthElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleOutlineWidth"));
            if (scaleOutlineWidthElem.First() != null)
            {
                string? scaleOutlineWidth = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleOutlineWidth").Value).FirstOrDefault();
                ScaleOutlineWidth = int.Parse(scaleOutlineWidth);
            }

            IEnumerable<XElement> scaleOutlineColorElem = mapScaleDoc.Descendants(ns + "ScaleOutlineColor");
            if (scaleOutlineColorElem.First() != null)
            {
                string? scaleOutlineColor = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleOutlineColor").Value).FirstOrDefault();
                ScaleOutlineColor = ColorTranslator.FromHtml(scaleOutlineColor);
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

            writer.WriteStartElement("ScaleGuid");
            writer.WriteString(ScaleGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleSegmentCount");
            writer.WriteValue(ScaleSegmentCount);
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleLineWidth");
            writer.WriteValue(ScaleLineWidth);
            writer.WriteEndElement();

            XmlColor scaleColor1 = new(ScaleColor1);
            writer.WriteStartElement("ScaleColor1");
            scaleColor1.WriteXml(writer);
            writer.WriteEndElement();

            XmlColor scaleColor2 = new(ScaleColor2);
            writer.WriteStartElement("ScaleColor2");
            scaleColor2.WriteXml(writer);
            writer.WriteEndElement();

            XmlColor scaleColor3 = new(ScaleColor3);
            writer.WriteStartElement("ScaleColor3");
            scaleColor3.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleDistance");
            writer.WriteValue(ScaleDistance);
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(ScaleDistanceUnit))
            {
                writer.WriteStartElement("ScaleDistanceUnit");
                writer.WriteString(ScaleDistanceUnit);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("ScaleNumbersDisplayType");
            writer.WriteString(ScaleNumbersDisplayType.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleFont");
            FontConverter cvt = new();
            string? s = cvt.ConvertToString(ScaleFont);
            if (!string.IsNullOrEmpty(s))
            {
                writer.WriteValue(s);
            }
            writer.WriteEndElement();

            XmlColor scaleFontColor = new(ScaleFontColor);
            writer.WriteStartElement("ScaleFontColor");
            scaleFontColor.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleOutlineWidth");
            writer.WriteValue(ScaleOutlineWidth);
            writer.WriteEndElement();

            XmlColor scaleOutlineColor = new(ScaleOutlineColor);
            writer.WriteStartElement("ScaleOutlineColor");
            scaleOutlineColor.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
