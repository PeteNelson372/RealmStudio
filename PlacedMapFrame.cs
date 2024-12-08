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
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class PlacedMapFrame : MapComponent, IXmlSerializable
    {
        public Guid FrameGuid = Guid.NewGuid();
        public SKBitmap? FrameBitmap { get; set; } = null;
        public SKBitmap? Patch_A { get; set; } = null;  // top left corner
        public SKBitmap? Patch_B { get; set; } = null; // top middle
        public SKBitmap? Patch_C { get; set; } = null; // top right corner
        public SKBitmap? Patch_D { get; set; } = null; // left side
        public SKBitmap? Patch_E { get; set; } = null; // middle
        public SKBitmap? Patch_F { get; set; } = null; // right side
        public SKBitmap? Patch_G { get; set; } = null; // bottom left corner
        public SKBitmap? Patch_H { get; set; } = null; // bottom middle
        public SKBitmap? Patch_I { get; set; } = null; // bottom right corner

        public bool FrameEnabled { get; set; } = true;

        public Color FrameTint { get; set; } = Color.WhiteSmoke;

        public float FrameScale { get; set; } = 1.0F;

        public SKPaint? FramePaint { get; set; } = null;

        public float FrameCenterLeft { get; set; } = 0;
        public float FrameCenterTop { get; set; } = 0;
        public float FrameCenterRight { get; set; } = 0;
        public float FrameCenterBottom { get; set; } = 0;

        public override void Render(SKCanvas canvas)
        {
            if (!FrameEnabled)
            {
                return;
            }

            //using (new SKAutoCanvasRestore(canvas))
            {
                if (FrameBitmap != null
                && Patch_A != null
                && Patch_B != null
                && Patch_C != null
                && Patch_D != null
                && Patch_F != null
                && Patch_G != null
                && Patch_H != null
                && Patch_I != null)
                {
                    using SKPaint framePaint = new()
                    {
                        Style = SKPaintStyle.Fill,
                        ColorFilter = SKColorFilter.CreateBlendMode(
                        Extensions.ToSKColor(FrameTint),
                        SKBlendMode.Modulate) // combine the tint with the bitmap color
                    };

                    FramePaint = framePaint.Clone();

                    // frame top
                    canvas.DrawBitmap(Patch_A, 0, 0, FramePaint);

                    // tile Patch_B - has to be an integral number of tiles;
                    float patch_b_tile_length = Width - Patch_A.Width - Patch_C.Width;

                    int num_patch_b_tiles = Math.Max((int)Math.Floor(patch_b_tile_length / Patch_B.Width), 1);

                    int newWidth = (int)Math.Ceiling(patch_b_tile_length / num_patch_b_tiles);

                    num_patch_b_tiles = (int)Math.Round((float)patch_b_tile_length / newWidth);

                    while ((newWidth * num_patch_b_tiles) + Patch_A.Width + Patch_C.Width < Width)
                    {
                        newWidth++;
                    }

                    // scale Patch_B so that it tiles an integral number of times
                    using SKBitmap scaled_B = new(newWidth, Patch_B.Height);
                    Patch_B.ScalePixels(scaled_B, SKFilterQuality.High);

                    int left = Patch_A.Width;

                    for (int i = 0; i < num_patch_b_tiles; i++)
                    {
                        canvas.DrawBitmap(scaled_B, left, 0, FramePaint);
                        left += scaled_B.Width;
                    }
                    canvas.DrawBitmap(Patch_C, Width - Patch_C.Width, 0, FramePaint);

                    // tile Patch_D - has to be an integral number of tiles;
                    float patch_d_tile_height = Height - Patch_A.Height - Patch_G.Height;

                    int num_patch_d_tiles = Math.Max((int)(patch_d_tile_height / Patch_D.Height), 1);

                    int newHeight = (int)Math.Ceiling(patch_d_tile_height / num_patch_d_tiles);

                    num_patch_d_tiles = (int)Math.Round((float)patch_d_tile_height / newHeight);

                    while ((newHeight * num_patch_d_tiles) + Patch_A.Height + Patch_G.Height < Height)
                    {
                        newHeight++;
                    }

                    // scale Patch_D so that it tiles an integral number of times
                    // scaled D patch has same width and new height
                    using SKBitmap scaled_D = new(Patch_D.Width, newHeight);
                    Patch_D.ScalePixels(scaled_D, SKFilterQuality.High);

                    int top = Patch_A.Height;

                    for (int i = 0; i < num_patch_d_tiles; i++)
                    {
                        canvas.DrawBitmap(scaled_D, 0, top, FramePaint);
                        top += scaled_D.Height;
                    }

                    float patch_f_tile_height = Height - Patch_C.Height - Patch_I.Height;

                    int num_patch_f_tiles = Math.Max((int)(patch_f_tile_height / Patch_F.Height), 1);

                    int newFHeight = (int)Math.Ceiling(patch_f_tile_height / num_patch_f_tiles);

                    num_patch_f_tiles = (int)Math.Round((float)patch_f_tile_height / newHeight);

                    while ((newFHeight * num_patch_f_tiles) + Patch_C.Height + Patch_I.Height < Height)
                    {
                        newFHeight++;
                    }

                    // scale Patch_F so that it tiles an integral number of times
                    using SKBitmap scaled_F = new(Patch_F.Width, newFHeight);
                    Patch_F.ScalePixels(scaled_F, SKFilterQuality.High);

                    top = Patch_C.Height;

                    for (int i = 0; i < num_patch_f_tiles; i++)
                    {
                        canvas.DrawBitmap(scaled_F, Width - scaled_F.Width, top, FramePaint);
                        top += scaled_F.Height;
                    }

                    // frame bottom
                    canvas.DrawBitmap(Patch_G, 0, Height - Patch_G.Height, FramePaint);

                    // scale Patch_H so that it tiles an integral number of times
                    float patch_h_tile_length = Width - Patch_G.Width - Patch_I.Width;

                    int num_patch_h_tiles = Math.Max((int)Math.Floor(patch_h_tile_length / Patch_H.Width), 1);

                    int newHWidth = (int)Math.Ceiling(patch_h_tile_length / num_patch_h_tiles);

                    num_patch_h_tiles = (int)Math.Round((float)patch_h_tile_length / newWidth);

                    while ((newHWidth * num_patch_h_tiles) + Patch_G.Width + Patch_I.Width < Width)
                    {
                        newHWidth++;
                    }

                    using SKBitmap scaled_H = new(newHWidth, Patch_H.Height);
                    Patch_H.ScalePixels(scaled_H, SKFilterQuality.High);

                    left = Patch_G.Width;

                    // patch H is tiled the same number of times as patch B
                    for (int i = 0; i < num_patch_h_tiles; i++)
                    {
                        canvas.DrawBitmap(scaled_H, left, Height - Patch_H.Height, FramePaint);
                        left += scaled_H.Width;
                    }

                    canvas.DrawBitmap(Patch_I, Width - Patch_I.Width, Height - Patch_I.Height, FramePaint);
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
            XDocument mapFrameDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapFrameDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapFrameDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapFrameDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapFrameDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> guidElemEnum = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameGuid"));
            if (guidElemEnum.First() != null)
            {
                string? frameGuid = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameGuid").Value).FirstOrDefault();
                FrameGuid = Guid.Parse(frameGuid);
            }

            string? base64String = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameBitmap").Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(base64String))
            {
                // Convert Base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Create an image from the byte array
                using (MemoryStream ms = new(imageBytes))
                {
                    FrameBitmap = SKBitmap.Decode(ms);
                }
            }

            IEnumerable<XElement> frameTintElem = mapFrameDoc.Descendants(ns + "FrameTint");
            if (frameTintElem.First() != null)
            {
                string? frameTint = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameTint").Value).FirstOrDefault();
                FrameTint = ColorTranslator.FromHtml(frameTint);
            }

            IEnumerable<XElement?> frameScaleElem = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameScale"));
            if (frameScaleElem.First() != null)
            {
                string? frameScale = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameScale").Value).FirstOrDefault();
                FrameScale = float.Parse(frameScale);
            }

            IEnumerable<XElement?> frameCenterLeftElem = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterLeft"));
            if (frameCenterLeftElem.First() != null)
            {
                string? frameCenterLeft = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterLeft").Value).FirstOrDefault();
                FrameCenterLeft = float.Parse(frameCenterLeft);
            }

            IEnumerable<XElement?> frameCenterTopElem = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterTop"));
            if (frameCenterTopElem.First() != null)
            {
                string? frameCenterTop = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterTop").Value).FirstOrDefault();
                FrameCenterTop = float.Parse(frameCenterTop);
            }

            IEnumerable<XElement?> frameCenterRightElem = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterRight"));
            if (frameCenterRightElem.First() != null)
            {
                string? frameCenterRight = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterRight").Value).FirstOrDefault();
                FrameCenterRight = float.Parse(frameCenterRight);
            }

            IEnumerable<XElement?> frameCenterBottomElem = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterBottom"));
            if (frameCenterBottomElem.First() != null)
            {
                string? frameCenterBottom = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameCenterBottom").Value).FirstOrDefault();
                FrameCenterBottom = float.Parse(frameCenterBottom);
            }

            OverlayMethods.CompletePlacedFrame(this);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            // frame GUID
            writer.WriteStartElement("FrameGuid");
            writer.WriteString(FrameGuid.ToString());
            writer.WriteEndElement();

            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);
            FrameBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
            byte[] bitmapData = ms.ToArray();
            writer.WriteStartElement("FrameBitmap");
            writer.WriteBase64(bitmapData, 0, bitmapData.Length);
            writer.WriteEndElement();

            XmlColor frametint = new(FrameTint);
            writer.WriteStartElement("FrameTint");
            frametint.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("FrameScale");
            writer.WriteValue(FrameScale);
            writer.WriteEndElement();

            writer.WriteStartElement("FrameCenterLeft");
            writer.WriteValue(FrameCenterLeft);
            writer.WriteEndElement();

            writer.WriteStartElement("FrameCenterTop");
            writer.WriteValue(FrameCenterTop);
            writer.WriteEndElement();

            writer.WriteStartElement("FrameCenterRight");
            writer.WriteValue(FrameCenterRight);
            writer.WriteEndElement();

            writer.WriteStartElement("FrameCenterBottom");
            writer.WriteValue(FrameCenterBottom);
            writer.WriteEndElement();
        }
    }
}
