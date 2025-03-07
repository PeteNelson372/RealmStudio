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
    public class MapSymbol : MapComponent, IXmlSerializable
    {
        public string Name { get; set; } = string.Empty;

        public Guid SymbolGuid { get; set; } = Guid.NewGuid();

        public int SymbolWidth { get; set; }

        public int SymbolHeight { get; set; }

        public bool IsSelected { get; set; }

        public SymbolFormatEnum SymbolFormat { get; set; } = SymbolFormatEnum.NotSet;

        public SymbolTypeEnum SymbolType { get; set; } = SymbolTypeEnum.NotSet;

        public List<string> SymbolTags { get; set; } = [];

        public string SymbolName { get; set; } = string.Empty;

        public string CollectionName { get; set; } = string.Empty;

        public string CollectionPath { get; set; } = string.Empty;

        public string SymbolFilePath { get; set; } = string.Empty;

        public bool IsGrayscale { get; set; }

        public bool UseCustomColors { get; set; }

        public SKColor[] CustomSymbolColors { get; set; } = new SKColor[3];

        public SKPaint? SymbolPaint { get; set; }

        public SKBitmap? SymbolBitmap { get; set; }

        public SKBitmap? ColorMappedBitmap { get; set; }

        public SKBitmap? PlacedBitmap { get; set; }

        public string SymbolSVG { get; set; } = string.Empty;

        public MapSymbol() { }

        public MapSymbol(MapSymbol original)
        {
            Name = original.Name;
            IsGrayscale = original.IsGrayscale;
            UseCustomColors = original.UseCustomColors;

            for (int i = 0; i < CustomSymbolColors.Length; i++)
            {
                CustomSymbolColors[i] = original.CustomSymbolColors[i];
            }

            IsSelected = original.IsSelected;
            SymbolFormat = original.SymbolFormat;
            CollectionName = original.CollectionName;
            PlacedBitmap = original.PlacedBitmap?.Copy();
            SymbolBitmap = original.SymbolBitmap?.Copy();
            SymbolFilePath = original.SymbolFilePath;
            SymbolName = original.SymbolName;
            SymbolSVG = original.SymbolSVG;
            SymbolType = original.SymbolType;
            SymbolTags = [.. original.SymbolTags];
            Width = original.Width;
            Height = original.Height;
            X = original.X;
            Y = original.Y;

            SymbolWidth = original.SymbolWidth;
            SymbolHeight = original.SymbolHeight;
            SymbolPaint = original.SymbolPaint?.Clone();
        }

        public void ClearSymbolTags()
        {
            SymbolTags.Clear();
        }

        public void AddSymbolTag(string tag)
        {
            if (SymbolTags.Contains(tag)) return;
            SymbolTags.Add(tag);
        }

        public void SetSymbolBitmapFromPath(string path)
        {
            if (File.Exists(path))
            {
                SymbolBitmap?.Dispose();
                SymbolBitmap = SKBitmap.Decode(path);
            }
        }

        public void SetPlacedBitmap(SKBitmap placedBitmap)
        {
            PlacedBitmap = placedBitmap;
            Width = PlacedBitmap.Width;
            Height = PlacedBitmap.Height;
        }

        public override void Render(SKCanvas canvas)
        {
            if (PlacedBitmap != null)
            {
                SKPoint point = new(X, Y);

                if (IsGrayscale)
                {
                    canvas.DrawBitmap(PlacedBitmap, point, SymbolPaint);
                }
                else
                {
                    canvas.DrawBitmap(PlacedBitmap, point, null);
                }

                if (IsSelected)
                {
                    canvas.DrawRect(new SKRect(Math.Max(0, X - 1), Math.Max(0, Y - 1), X + Width + 1, Y + Height + 1), PaintObjects.MapSymbolSelectPaint);
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
            XDocument mapSymbolDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapSymbolDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapSymbolDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapSymbolDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapSymbolDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> wElemEnum = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolWidth"));
            if (wElemEnum.First() != null)
            {
                string? symboWidth = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolWidth").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(symboWidth))
                {
                    SymbolWidth = int.Parse(symboWidth);
                }
            }

            IEnumerable<XElement?> hElemEnum = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolHeight"));
            if (hElemEnum.First() != null)
            {
                string? symbolHeight = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolHeight").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(symbolHeight))
                {
                    SymbolHeight = int.Parse(symbolHeight);
                }
            }

            string? symbolFormat = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolFormat").Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(symbolFormat))
            {
                SymbolFormat = Enum.Parse<SymbolFormatEnum>(symbolFormat);
            }

            string? symbolType = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolType").Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(symbolFormat))
            {
                SymbolType = Enum.Parse<SymbolTypeEnum>(symbolType);
            }

            IEnumerable<XElement> tagElem = mapSymbolDoc.Descendants(ns + "Tag");
            foreach (XElement tag in tagElem)
            {
                string tagValue = tag.Value;
                if (!string.IsNullOrEmpty(tagValue))
                {
                    SymbolTags.Add(tagValue);
                }
            }

            string? symbolGuid = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolGuid").Value).FirstOrDefault();
            SymbolGuid = Guid.Parse(symbolGuid);

            string? symbolName = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolName").Value).FirstOrDefault();
            SymbolName = symbolName;

            string? collectionName = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "CollectionName").Value).FirstOrDefault();
            CollectionName = collectionName;

            IEnumerable<XElement?> cpElemEnum = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "CollectionPath"));
            if (cpElemEnum.First() != null)
            {
                string? collectionPath = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "CollectionPath").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(CollectionPath))
                {
                    CollectionPath = collectionPath;
                }
            }

            string? symbolFilePath = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolFilePath").Value).FirstOrDefault();
            SymbolFilePath = symbolFilePath;

            string? isGrayscale = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "IsGrayscale").Value).FirstOrDefault();
            IsGrayscale = bool.Parse(isGrayscale);

            string? useCustomColors = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "UseCustomColors").Value).FirstOrDefault();
            UseCustomColors = bool.Parse(useCustomColors);

            IEnumerable<XElement> customColorElem = mapSymbolDoc.Descendants(ns + "CustomColors");
            if (customColorElem != null && customColorElem.Any() && customColorElem.First() != null)
            {
                List<XElement> elemList = [.. customColorElem.Descendants()];

                if (elemList != null && elemList.Count == 3)
                {
                    XElement colorElem = elemList[0];
                    string colorString = colorElem.Value;

                    if (colorString.StartsWith('#'))
                    {
                        CustomSymbolColors[0] = ColorTranslator.FromHtml(colorString).ToSKColor();
                    }
                    else
                    {
                        if (int.TryParse(colorString, out int colorArgb))
                        {
                            CustomSymbolColors[0] = Color.FromArgb(colorArgb).ToSKColor();
                        }
                        else
                        {
                            CustomSymbolColors[0] = Color.FromArgb(255, 85, 44, 36).ToSKColor();
                        }
                    }

                    CustomSymbolColors[0] = Extensions.ToSKColor(ColorTranslator.FromHtml(colorString));

                    colorElem = elemList[1];
                    colorString = colorElem.Value;

                    if (colorString.StartsWith('#'))
                    {
                        CustomSymbolColors[1] = ColorTranslator.FromHtml(colorString).ToSKColor();
                    }
                    else
                    {
                        if (int.TryParse(colorString, out int colorArgb))
                        {
                            CustomSymbolColors[1] = Color.FromArgb(colorArgb).ToSKColor();
                        }
                        else
                        {
                            CustomSymbolColors[1] = Color.FromArgb(255, 53, 45, 32).ToSKColor();
                        }
                    }

                    colorElem = elemList[2];
                    colorString = colorElem.Value;

                    if (colorString.StartsWith('#'))
                    {
                        CustomSymbolColors[2] = ColorTranslator.FromHtml(colorString).ToSKColor();
                    }
                    else
                    {
                        if (int.TryParse(colorString, out int colorArgb))
                        {
                            CustomSymbolColors[2] = Color.FromArgb(colorArgb).ToSKColor();
                        }
                        else
                        {
                            CustomSymbolColors[2] = Color.FromArgb(161, 214, 202, 171).ToSKColor();
                        }
                    }
                }
                else
                {
                    CustomSymbolColors[0] = Color.FromArgb(255, 85, 44, 36).ToSKColor();
                    CustomSymbolColors[1] = Color.FromArgb(255, 53, 45, 32).ToSKColor();
                    CustomSymbolColors[2] = Color.FromArgb(161, 214, 202, 171).ToSKColor();
                }
            }
            else
            {
                CustomSymbolColors[0] = Color.FromArgb(255, 85, 44, 36).ToSKColor();
                CustomSymbolColors[1] = Color.FromArgb(255, 53, 45, 32).ToSKColor();
                CustomSymbolColors[2] = Color.FromArgb(161, 214, 202, 171).ToSKColor();
            }

            IEnumerable<XElement?> sbmElemEnum = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolBitmap"));
            if (sbmElemEnum.First() != null)
            {
                string? symbolBitmapBase64String = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "SymbolBitmap").Value).FirstOrDefault();
                if (!string.IsNullOrEmpty(symbolBitmapBase64String))
                {
                    // Convert Base64 string to byte array
                    byte[] imageBytes = Convert.FromBase64String(symbolBitmapBase64String);

                    // Create an image from the byte array
                    using MemoryStream ms = new(imageBytes);
                    SymbolBitmap = SKBitmap.Decode(ms);
                }
            }

            IEnumerable<XElement?> cmbmElemEnum = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "ColorMappedBitmap"));
            if (cmbmElemEnum.First() != null)
            {
                string? colorMappedBitmapBase64String = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "ColorMappedBitmap").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(colorMappedBitmapBase64String))
                {
                    // Convert Base64 string to byte array
                    byte[] imageBytes = Convert.FromBase64String(colorMappedBitmapBase64String);

                    // Create an image from the byte array
                    using MemoryStream ms = new(imageBytes);
                    ColorMappedBitmap = SKBitmap.Decode(ms);
                }
            }

            IEnumerable<XElement?> plbmElemEnum = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "PlacedBitmap"));
            if (plbmElemEnum.First() != null)
            {
                string? placedBitmapBase64String = mapSymbolDoc.Descendants().Select(x => x.Element(ns + "PlacedBitmap").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(placedBitmapBase64String))
                {
                    // Convert Base64 string to byte array
                    byte[] imageBytes = Convert.FromBase64String(placedBitmapBase64String);

                    // Create an image from the byte array
                    using MemoryStream ms = new(imageBytes);
                    PlacedBitmap = SKBitmap.Decode(ms);
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

            writer.WriteStartElement("SymbolWidth");
            writer.WriteValue(SymbolWidth);
            writer.WriteEndElement();

            writer.WriteStartElement("SymbolHeight");
            writer.WriteValue(SymbolHeight);
            writer.WriteEndElement();

            // symbol format
            writer.WriteStartElement("SymbolFormat");
            writer.WriteString(SymbolFormat.ToString());
            writer.WriteEndElement();

            // symbol type
            writer.WriteStartElement("SymbolType");
            writer.WriteString(SymbolType.ToString());
            writer.WriteEndElement();

            // map path points
            writer.WriteStartElement("SymbolTags");
            foreach (string tag in SymbolTags)
            {
                writer.WriteStartElement("Tag");
                writer.WriteString($"{tag}");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("SymbolGuid");
            writer.WriteString(SymbolGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("SymbolName");
            writer.WriteString(SymbolName);
            writer.WriteEndElement();

            writer.WriteStartElement("CollectionName");
            writer.WriteString(CollectionName);
            writer.WriteEndElement();

            writer.WriteStartElement("CollectionPath");
            writer.WriteString(CollectionPath);
            writer.WriteEndElement();

            writer.WriteStartElement("SymbolFilePath");
            writer.WriteString(SymbolFilePath);
            writer.WriteEndElement();

            writer.WriteStartElement("IsGrayscale");
            writer.WriteValue(IsGrayscale);
            writer.WriteEndElement();

            writer.WriteStartElement("UseCustomColors");
            writer.WriteValue(UseCustomColors);
            writer.WriteEndElement();

            writer.WriteStartElement("CustomColors");
            for (int i = 0; i < CustomSymbolColors.Length; i++)
            {
                writer.WriteStartElement("CustomColor" + i.ToString());
                writer.WriteValue(CustomSymbolColors[i].ToDrawingColor().ToArgb());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            if (SymbolBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);
                SymbolBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] symbolBitmapData = ms.ToArray();
                writer.WriteStartElement("SymbolBitmap");
                writer.WriteBase64(symbolBitmapData, 0, symbolBitmapData.Length);
                writer.WriteEndElement();
            }

            if (ColorMappedBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);
                ColorMappedBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] colorMappedBitmapData = ms.ToArray();
                writer.WriteStartElement("ColorMappedBitmap");
                writer.WriteBase64(colorMappedBitmapData, 0, colorMappedBitmapData.Length);
                writer.WriteEndElement();
            }

            if (PlacedBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);
                PlacedBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] placedBitmapData = ms.ToArray();
                writer.WriteStartElement("PlacedBitmap");
                writer.WriteBase64(placedBitmapData, 0, placedBitmapData.Length);
                writer.WriteEndElement();
            }
        }
    }
}
