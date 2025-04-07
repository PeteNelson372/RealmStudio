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
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class MapScale : MapComponent, IXmlSerializable, IDisposable
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
        public ScaleNumbersDisplayLocation ScaleNumbersDisplayType { get; set; } = ScaleNumbersDisplayLocation.All;  // where to display the segment labels
        public Font ScaleFont { get; set; } = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0); // scale segment label font, color, outline
        public Color ScaleFontColor { get; set; } = Color.White;
        public int ScaleOutlineWidth { get; set; } = 2;
        public Color ScaleOutlineColor { get; set; } = Color.Black;
        public bool IsSelected { get; set; }

        private SKPaint SegmentOutlinePaint = new();
        private SKPaint EvenSegmentPaint = new();
        private SKPaint OddSegmentPaint = new();
        private SKPaint ScaleLabelPaint = new();
        private SKPaint OutlinePaint = new();
        private bool disposedValue;

        public MapScale() { }

        public MapScale(MapScale mapScale)
        {
            X = mapScale.X;
            Y = mapScale.Y;
            Width = mapScale.Width;
            Height = mapScale.Height;
            ScaleSegmentCount = mapScale.ScaleSegmentCount;
            ScaleLineWidth = mapScale.ScaleLineWidth;
            ScaleColor1 = mapScale.ScaleColor1;
            ScaleColor2 = mapScale.ScaleColor2;
            ScaleColor3 = mapScale.ScaleColor3;
            ScaleDistance = mapScale.ScaleDistance;
            ScaleDistanceUnit = mapScale.ScaleDistanceUnit;
            ScaleFontColor = mapScale.ScaleFontColor;
            ScaleOutlineWidth = mapScale.ScaleOutlineWidth;
            ScaleOutlineColor = mapScale.ScaleOutlineColor;
            ScaleFont = (Font)mapScale.ScaleFont.Clone();
            ScaleNumbersDisplayType = mapScale.ScaleNumbersDisplayType;
        }

        private void ConstructPaintObjects()
        {
            SegmentOutlinePaint.Dispose();
            SegmentOutlinePaint = new()
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = ScaleLineWidth,
                Color = ScaleColor3.ToSKColor()
            };

            EvenSegmentPaint.Dispose();
            EvenSegmentPaint = new()
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = Height - ScaleLineWidth,
                Color = ScaleColor1.ToSKColor()
            };

            OddSegmentPaint.Dispose();
            OddSegmentPaint = new()
            {
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = Height - ScaleLineWidth,
                Color = ScaleColor2.ToSKColor(),
            };

            ScaleLabelPaint.Dispose();
            ScaleLabelPaint = new()
            {
                Color = Extensions.ToSKColor(ScaleFontColor),
                IsAntialias = true
            };

            OutlinePaint.Dispose();
            OutlinePaint = new()
            {
                Color = ScaleOutlineColor.ToSKColor(),
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDilate(ScaleOutlineWidth, ScaleOutlineWidth),
            };
        }

        public override void Render(SKCanvas canvas)
        {
            float segmentWidth = Width / ScaleSegmentCount;

            ConstructPaintObjects();

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

            SKTypeface fontTypeface = SKTypeface.FromFamilyName(ScaleFont.FontFamily.Name, fs);

            SKFont skScaleFont = new(fontTypeface)
            {
                Size = ScaleFont.Size * 1.33F
            };

            // draw the scale outline
            SKRect outlineRect = new(X, Y, X + Width, Y + Height);

            canvas.DrawRect(outlineRect, SegmentOutlinePaint);

            // draw the segments and labels
            int segmentX = X + (ScaleLineWidth / 2);
            int segmentY = Y + (Height / 2);

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

                skScaleFont.MeasureText(distanceText, out SKRect bounds, ScaleLabelPaint);

                SKPoint labelPoint = new(labelX, labelY - bounds.Height);

                switch (ScaleNumbersDisplayType)
                {
                    case ScaleNumbersDisplayLocation.Ends:
                        {
                            if (i == 0 || i == ScaleSegmentCount)
                            {
                                canvas.DrawText(distanceText, labelPoint, skScaleFont, OutlinePaint);
                                canvas.DrawText(distanceText, labelPoint, skScaleFont, ScaleLabelPaint);
                            }
                        }
                        break;
                    case ScaleNumbersDisplayLocation.EveryOther:
                        {
                            if (int.IsEvenInteger(i))
                            {
                                canvas.DrawText(distanceText, labelPoint, skScaleFont, OutlinePaint);
                                canvas.DrawText(distanceText, labelPoint, skScaleFont, ScaleLabelPaint);
                            }
                        }
                        break;
                    case ScaleNumbersDisplayLocation.All:
                        {
                            canvas.DrawText(distanceText, labelPoint, skScaleFont, OutlinePaint);
                            canvas.DrawText(distanceText, labelPoint, skScaleFont, ScaleLabelPaint);
                        }
                        break;
                }

                distance += ScaleDistance;
                labelX = (int)(labelX + segmentWidth);
            }

            if (!string.IsNullOrEmpty(ScaleDistanceUnit))
            {
                // draw the scale unit label
                skScaleFont.MeasureText(ScaleDistanceUnit, out SKRect bounds, ScaleLabelPaint);

                int unitLabelX = (int)(X + (Width / 2) - (bounds.Width / 2.0F));
                int unitLabelY = Y + Height;

                unitLabelY = (int)(unitLabelY + bounds.Height * 2);

                SKPoint unitLabelPoint = new(unitLabelX, unitLabelY);
                canvas.DrawText(ScaleDistanceUnit, unitLabelPoint, skScaleFont, OutlinePaint);
                canvas.DrawText(ScaleDistanceUnit, unitLabelPoint, skScaleFont, ScaleLabelPaint);
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
            if (scaleColor1Elem != null && scaleColor1Elem.Any() && scaleColor1Elem.First() != null)
            {
                string? scaleColor1 = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleColor1").Value).FirstOrDefault();

                int argbValue = 0;

                if (!string.IsNullOrEmpty(scaleColor1))
                {
                    if (scaleColor1.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(scaleColor1).ToArgb();
                    }
                    else if (int.TryParse(scaleColor1, out int n))
                    {
                        argbValue = n;
                    }

                    ScaleColor1 = Color.FromArgb(argbValue);
                }
                else
                {
                    ScaleColor1 = Color.Black;
                }
            }

            IEnumerable<XElement> scaleColor2Elem = mapScaleDoc.Descendants(ns + "ScaleColor2");
            if (scaleColor2Elem != null && scaleColor2Elem.Any() && scaleColor2Elem.First() != null)
            {
                string? scaleColor2 = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleColor2").Value).FirstOrDefault();

                int argbValue = 0;

                if (!string.IsNullOrEmpty(scaleColor2))
                {
                    if (scaleColor2.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(scaleColor2).ToArgb();
                    }
                    else if (int.TryParse(scaleColor2, out int n))
                    {
                        argbValue = n;
                    }

                    ScaleColor2 = Color.FromArgb(argbValue);
                }
                else
                {
                    ScaleColor2 = Color.White;
                }
            }

            IEnumerable<XElement> scaleColor3Elem = mapScaleDoc.Descendants(ns + "ScaleColor3");
            if (scaleColor3Elem != null && scaleColor3Elem.Any() && scaleColor3Elem.First() != null)
            {
                string? scaleColor3 = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleColor3").Value).FirstOrDefault();

                int argbValue = 0;

                if (!string.IsNullOrEmpty(scaleColor3))
                {
                    if (scaleColor3.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(scaleColor3).ToArgb();
                    }
                    else if (int.TryParse(scaleColor3, out int n))
                    {
                        argbValue = n;
                    }

                    ScaleColor3 = Color.FromArgb(argbValue);
                }
                else
                {
                    ScaleColor3 = Color.Black;
                }
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
                ScaleNumbersDisplayType = Enum.Parse<ScaleNumbersDisplayLocation>(scaleNumbersDisplayType);
            }

            // font
            IEnumerable<XElement?> scaleFontElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleFont"));
            if (scaleFontElem != null && scaleFontElem.Any() && scaleFontElem.First() != null)
            {
                string? scaleFontString = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleFont").Value).FirstOrDefault();
                FontConverter cvt = new();
                Font? scaleFont = (Font?)cvt.ConvertFromString(scaleFontString) as Font;

                if (scaleFont != null && !scaleFontString.Contains(scaleFont.FontFamily.Name))
                {
                    scaleFont = null;
                }

                if (scaleFont == null)
                {
                    string[] fontParts = scaleFontString.Split(',');

                    if (fontParts.Length == 2)
                    {
                        string ff = fontParts[0];
                        string fs = fontParts[1];

                        // remove any non-numeric characters from the font size string (but allow . and -)
                        fs = string.Join(",", new string(
                            [.. fs.Where(c => char.IsBetween(c, '0', '9') || c == '.' || c == '-' || char.IsWhiteSpace(c))]).Split((char[]?)null,
                            StringSplitOptions.RemoveEmptyEntries));

                        bool success = float.TryParse(fs, out float fontsize);

                        if (!success)
                        {
                            fontsize = 12.0F;
                        }

                        try
                        {
                            FontFamily family = new(ff);
                            scaleFont = new Font(family, fontsize, FontStyle.Regular, GraphicsUnit.Point);
                        }
                        catch
                        {
                            // couldn't create the font, so try to find it in the list of embedded fonts
                            for (int i = 0; i < LabelManager.EMBEDDED_FONTS.Families.Length; i++)
                            {
                                if (LabelManager.EMBEDDED_FONTS.Families[i].Name == ff)
                                {
                                    scaleFont = new Font(LabelManager.EMBEDDED_FONTS.Families[i], fontsize, FontStyle.Regular, GraphicsUnit.Point);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (scaleFont != null)
                {
                    ScaleFont = scaleFont;
                }
                else
                {
                    ScaleFont = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
                }
            }



            IEnumerable<XElement> scaleFontColorElem = mapScaleDoc.Descendants(ns + "ScaleFontColor");
            if (scaleFontColorElem != null && scaleFontColorElem.Any() && scaleFontColorElem.First() != null)
            {
                string? scaleFontColor = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleFontColor").Value).FirstOrDefault();

                int argbValue = 0;

                if (!string.IsNullOrEmpty(scaleFontColor))
                {
                    if (scaleFontColor.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(scaleFontColor).ToArgb();
                    }
                    else if (int.TryParse(scaleFontColor, out int n))
                    {
                        argbValue = n;
                    }

                    ScaleFontColor = Color.FromArgb(argbValue);
                }
                else
                {
                    ScaleFontColor = Color.White;
                }
            }

            IEnumerable<XElement?> scaleOutlineWidthElem = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleOutlineWidth"));
            if (scaleOutlineWidthElem != null && scaleOutlineWidthElem.Any() && scaleOutlineWidthElem.First() != null)
            {
                string? scaleOutlineWidth = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleOutlineWidth").Value).FirstOrDefault();

                if (int.TryParse(scaleOutlineWidth, out int n))
                {
                    if (n > 0)
                    {
                        ScaleOutlineWidth = n;
                    }
                    else
                    {
                        ScaleOutlineWidth = 2;
                    }
                }
                else
                {
                    ScaleOutlineWidth = 2;
                }
            }

            IEnumerable<XElement> scaleOutlineColorElem = mapScaleDoc.Descendants(ns + "ScaleOutlineColor");
            if (scaleOutlineColorElem != null && scaleOutlineColorElem.Any() && scaleOutlineColorElem.First() != null)
            {
                string? scaleOutlineColor = mapScaleDoc.Descendants().Select(x => x.Element(ns + "ScaleOutlineColor").Value).FirstOrDefault();

                int argbValue = 0;

                if (!string.IsNullOrEmpty(scaleOutlineColor))
                {
                    if (scaleOutlineColor.StartsWith('#'))
                    {
                        argbValue = ColorTranslator.FromHtml(scaleOutlineColor).ToArgb();
                    }
                    else if (int.TryParse(scaleOutlineColor, out int n))
                    {
                        argbValue = n;
                    }

                    ScaleOutlineColor = Color.FromArgb(argbValue);
                }
                else
                {
                    ScaleOutlineColor = Color.Black;
                }
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

            writer.WriteStartElement("ScaleColor1");
            writer.WriteValue(ScaleColor1.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleColor2");
            writer.WriteValue(ScaleColor2.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleColor3");
            writer.WriteValue(ScaleColor3.ToArgb());
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

            writer.WriteStartElement("ScaleFontColor");
            writer.WriteValue(ScaleFontColor.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleOutlineWidth");
            writer.WriteValue(ScaleOutlineWidth);
            writer.WriteEndElement();

            writer.WriteStartElement("ScaleOutlineColor");
            writer.WriteValue(ScaleOutlineColor.ToArgb());
            writer.WriteEndElement();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SegmentOutlinePaint.Dispose();
                    EvenSegmentPaint.Dispose();
                    OddSegmentPaint.Dispose();
                    ScaleLabelPaint.Dispose();
                    OutlinePaint.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
