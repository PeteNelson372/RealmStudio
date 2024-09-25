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

namespace RealmStudio
{
    internal class MapLabel : MapComponent, IXmlSerializable
    {
        public bool IsSelected { get; set; } = false;

        public Guid LabelGuid { get; set; } = Guid.NewGuid();

        public string LabelText { get; set; } = "";

        public Color LabelColor { get; set; } = ColorTranslator.FromHtml("#3D351E");

        public Color LabelOutlineColor { get; set; } = ColorTranslator.FromHtml("#A1D6CAAB");

        public int LabelOutlineWidth { get; set; } = 0;

        public Color LabelGlowColor { get; set; } = Color.White;

        public int LabelGlowStrength { get; set; } = 0;

        public Font LabelFont { get; set; } = new Font("Segoe", 9.0F, FontStyle.Regular, GraphicsUnit.Point, 0);

        public float LabelRotationDegrees { get; set; } = 0.0F;

        public SKFont LabelSKFont { get; set; } = new SKFont(); // doesn't need to be serialized

        public SKPath? LabelPath { get; set; } = null;

        public SKPaint? LabelPaint { get; set; } = null;

        public override void Render(SKCanvas canvas)
        {
            if (!string.IsNullOrEmpty(LabelText) && LabelPaint != null)
            {
                SKPoint point = new(X, Y + Height);

                if (LabelPath == null || LabelPath.PointCount < 3)
                {
                    using (new SKAutoCanvasRestore(canvas))
                    {
                        // do any transformations
                        canvas.RotateDegrees(LabelRotationDegrees, X + Width / 2, Y + Height / 2);

                        if (LabelGlowStrength > 0)
                        {
                            SKPaint glowPaint = LabelPaint.Clone();
                            glowPaint.Color = LabelGlowColor.ToSKColor();
                            glowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Solid, LabelGlowStrength, true);
                            canvas.DrawText(LabelText, point, glowPaint);
                        }

                        if (LabelOutlineWidth > 0)
                        {
                            SKPaint outlinePaint = LabelPaint.Clone();
                            outlinePaint.Color = LabelOutlineColor.ToSKColor();
                            outlinePaint.ImageFilter = SKImageFilter.CreateDilate(LabelOutlineWidth / 100, LabelOutlineWidth / 100);
                            canvas.DrawText(LabelText, point, outlinePaint);
                        }

                        // draw the text
                        canvas.DrawText(LabelText, point, LabelPaint);
                        // auto restore, even on exceptions or errors

                        if (IsSelected)
                        {
                            // draw box around label to show it is selected
                            SKRect selectRect = new(X, Y, X + Width, Y + Height);
                            canvas.DrawRect(selectRect, PaintObjects.LabelSelectPaint);
                        }
                    }
                }
                else
                {
                    using (new SKAutoCanvasRestore(canvas))
                    {
                        using SKTextBlob? sKTextBlob = SKTextBlob.CreatePathPositioned(LabelText, LabelSKFont, LabelPath, LabelPaint.TextAlign, new SKPoint(0, 0));
                        if (sKTextBlob != null)
                        {
                            SKRect boundsRect = sKTextBlob.Bounds;

                            X = (int)boundsRect.Left;
                            Y = (int)boundsRect.Top;
                            Width = (int)boundsRect.Width;
                            Height = (int)boundsRect.Height;

                            // do any transformations
                            canvas.RotateDegrees(LabelRotationDegrees, X + Width / 2, Y + Height / 2);

                            LabelPaint.TextAlign = SKTextAlign.Center;

                            if (LabelGlowStrength > 0)
                            {
                                SKPaint glowPaint = LabelPaint.Clone();
                                glowPaint.Color = LabelGlowColor.ToSKColor();
                                glowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Outer, LabelGlowStrength, true);
                                canvas.DrawTextOnPath(LabelText, LabelPath, new SKPoint(0, 0), false, glowPaint);
                            }

                            if (LabelOutlineWidth > 0)
                            {
                                SKPaint outlinePaint = LabelPaint.Clone();
                                outlinePaint.Color = LabelOutlineColor.ToSKColor();
                                outlinePaint.ImageFilter = SKImageFilter.CreateDilate(LabelOutlineWidth, LabelOutlineWidth);
                                canvas.DrawTextOnPath(LabelText, LabelPath, new SKPoint(0, 0), false, outlinePaint);
                            }

                            // draw the text
                            canvas.DrawTextOnPath(LabelText, LabelPath, new SKPoint(0, 0), false, LabelPaint);

                            if (IsSelected)
                            {
                                // draw box around label to show it is selected
                                canvas.DrawRect(boundsRect, PaintObjects.LabelSelectPaint);
                            }
                        }
                    }
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

            XNamespace ns = "MapCreator";
            string content = reader.ReadOuterXml();
            XDocument mapLabelDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapLabelDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapLabelDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapLabelDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapLabelDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> guidElemEnum = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelGuid"));
            if (guidElemEnum.First() != null)
            {
                string? labelGuid = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelGuid").Value).FirstOrDefault();
                LabelGuid = Guid.Parse(labelGuid);
            }

            IEnumerable<XElement?> labelTextElemEnum = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelText"));
            if (labelTextElemEnum.First() != null)
            {
                string? labelText = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelText").Value).FirstOrDefault();
                LabelText = labelText;
            }

            IEnumerable<XElement> labelColorElem = mapLabelDoc.Descendants(ns + "LabelColor");
            if (labelColorElem.First() != null)
            {
                string? labelColor = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelColor").Value).FirstOrDefault();
                LabelColor = ColorTranslator.FromHtml(labelColor);
            }

            IEnumerable<XElement> labelOutlineColorElem = mapLabelDoc.Descendants(ns + "LabelOutlineColor");
            if (labelOutlineColorElem.First() != null)
            {
                string? labelOutlineColor = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelOutlineColor").Value).FirstOrDefault();
                LabelOutlineColor = ColorTranslator.FromHtml(labelOutlineColor);
            }

            IEnumerable<XElement?> labelOutlineWidthElem = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelOutlineWidth"));
            if (labelOutlineWidthElem.First() != null)
            {
                string? labelOutlineWidth = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelOutlineWidth").Value).FirstOrDefault();
                LabelOutlineWidth = int.Parse(labelOutlineWidth);
            }

            IEnumerable<XElement> labelGlowColorElem = mapLabelDoc.Descendants(ns + "LabelGlowColor");
            if (labelGlowColorElem.First() != null)
            {
                string? labelGlowColor = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelGlowColor").Value).FirstOrDefault();
                LabelGlowColor = ColorTranslator.FromHtml(labelGlowColor);
            }

            IEnumerable<XElement?> labelGlowStrengthElem = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelGlowStrength"));
            if (labelGlowStrengthElem.First() != null)
            {
                string? labelGlowStrength = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelGlowStrength").Value).FirstOrDefault();
                LabelGlowStrength = int.Parse(labelGlowStrength);
            }

            IEnumerable<XElement?> labelFontElem = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelFont"));
            if (labelFontElem.First() != null)
            {
                string? labelFont = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelFont").Value).FirstOrDefault();
                FontConverter cvt = new();
                LabelFont = cvt.ConvertFromString(labelFont) as Font;

                if (LabelFont == null)
                {
                    LabelFont = new Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
                }
            }

            IEnumerable<XElement?> labelRotationDegreesElem = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelRotationDegrees"));
            if (labelRotationDegreesElem.First() != null)
            {
                string? labelRotationDegrees = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelRotationDegrees").Value).FirstOrDefault();
                LabelRotationDegrees = float.Parse(labelRotationDegrees);
            }

            IEnumerable<XElement?> pathElemEnum = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelPath"));
            if (pathElemEnum.First() != null)
            {
                string? labelPath = mapLabelDoc.Descendants().Select(x => x.Element(ns + "LabelPath").Value).FirstOrDefault();
                LabelPath = SKPath.ParseSvgPathData(labelPath);
            }

            SKPaint paint = MapLabelMethods.CreateLabelPaint(LabelFont, LabelColor, LabelTextAlignEnum.AlignCenter);
            SKFont paintFont = paint.ToFont();

            LabelPaint = paint;
            LabelSKFont?.Dispose();
            LabelSKFont = paintFont;

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

            writer.WriteStartElement("LabelGuid");
            writer.WriteString(LabelGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("LabelText");
            writer.WriteString(LabelText);
            writer.WriteEndElement();

            XmlColor labelColor = new(LabelColor);
            writer.WriteStartElement("LabelColor");
            labelColor.WriteXml(writer);
            writer.WriteEndElement();

            XmlColor labelOutlineColor = new(LabelOutlineColor);
            writer.WriteStartElement("LabelOutlineColor");
            labelOutlineColor.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("LabelOutlineWidth");
            writer.WriteValue(LabelOutlineWidth);
            writer.WriteEndElement();

            XmlColor labelGlowColor = new(LabelGlowColor);
            writer.WriteStartElement("LabelGlowColor");
            labelGlowColor.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("LabelGlowStrength");
            writer.WriteValue(LabelGlowStrength);
            writer.WriteEndElement();

            writer.WriteStartElement("LabelFont");
            FontConverter cvt = new();
            string? s = cvt.ConvertToString(LabelFont);
            if (!string.IsNullOrEmpty(s))
            {
                writer.WriteValue(s);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("LabelRotationDegrees");
            writer.WriteValue(LabelRotationDegrees);
            writer.WriteEndElement();

            if (LabelPath != null)
            {
                writer.WriteStartElement("LabelPath");
                string pathSvg = LabelPath.ToSvgPathData();
                writer.WriteValue(pathSvg);
                writer.WriteEndElement();
            }
        }
    }
}
