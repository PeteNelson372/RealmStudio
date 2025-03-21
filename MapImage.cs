﻿/**************************************************************************************************************************
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapImage : MapComponent, IXmlSerializable
    {
        public SKBitmap? MapImageBitmap { get; set; }
        public bool MirrorImage { get; set; }
        public bool UseShader { get; set; } = true;

        public MapImage() { }

        public override void Render(SKCanvas canvas)
        {
            if (MapImageBitmap != null)
            {
                if (UseShader)
                {
                    using SKPaint ImageFillPaint = new()
                    {
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true,
                        BlendMode = SKBlendMode.Src,
                    };

                    if (MirrorImage)
                    {
                        SKShader imgShader = SKShader.CreateBitmap(MapImageBitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                        ImageFillPaint.Shader = imgShader;
                    }
                    else
                    {
                        SKShader imgShader = SKShader.CreateBitmap(MapImageBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                        ImageFillPaint.Shader = imgShader;
                    }

                    canvas.DrawRect(X, Y, Width, Height, ImageFillPaint);
                }
                else
                {
                    canvas.DrawBitmap(MapImageBitmap, X, Y);
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

            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();
            XDocument mapBitmapDoc = XDocument.Parse(content);

            IEnumerable<XElement?> mapImageBitmapElem = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "Bitmap"));
            if (mapImageBitmapElem != null && mapImageBitmapElem.Any() && mapImageBitmapElem.First() != null)
            {
                string? base64String = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "Bitmap").Value).FirstOrDefault();

                if (!string.IsNullOrEmpty(base64String))
                {
                    try
                    {
                        // Convert Base64 string to byte array
                        byte[] imageBytes = Convert.FromBase64String(base64String);

                        // Create an image from the byte array
                        using MemoryStream ms = new(imageBytes);
                        MapImageBitmap = SKBitmap.Decode(ms);

                        Width = MapImageBitmap.Width;
                        Height = MapImageBitmap.Height;
                    }
                    catch { }
                }
            }

            IEnumerable<XElement?> mirrorImageElem = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "MirrorImage"));
            if (mirrorImageElem != null && mirrorImageElem.Any() && mirrorImageElem.First() != null)
            {
                string? mirrorImage = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "MirrorImage").Value).FirstOrDefault();

                if (bool.TryParse(mirrorImage, out bool boolMirror))
                {
                    MirrorImage = boolMirror;
                }
                else
                {
                    MirrorImage = false;
                }
            }
            else
            {
                MirrorImage = false;
            }

            IEnumerable<XElement?> useShaderElem = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "UseShader"));
            if (useShaderElem != null && useShaderElem.Any() && useShaderElem.First() != null)
            {
                string? useShader = mapBitmapDoc.Descendants().Select(x => x.Element(ns + "UseShader").Value).FirstOrDefault();

                if (bool.TryParse(useShader, out bool boolUseShader))
                {
                    UseShader = boolUseShader;
                }
                else
                {
                    UseShader = true;
                }
            }
            else
            {
                UseShader = true;
            }

#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            if (MapImageBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);

                MapImageBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] bitmapData = ms.ToArray();
                writer.WriteStartElement("Bitmap");
                writer.WriteBase64(bitmapData, 0, bitmapData.Length);
                writer.WriteEndElement();

                writer.WriteStartElement("MirrorImage");
                writer.WriteValue(MirrorImage);
                writer.WriteEndElement();

                writer.WriteStartElement("UseShader");
                writer.WriteValue(UseShader);
                writer.WriteEndElement();
            }
        }
    }
}
