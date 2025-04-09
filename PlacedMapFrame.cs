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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class PlacedMapFrame : MapComponent, IXmlSerializable
    {
        public Guid FrameGuid { get; set; } = Guid.NewGuid();
        public SKBitmap? FrameBitmap { get; set; }
        public SKBitmap? PatchA { get; set; }   // top left corner
        public SKBitmap? PatchB { get; set; }  // top middle
        public SKBitmap? PatchC { get; set; }  // top right corner
        public SKBitmap? PatchD { get; set; }  // left side
        public SKBitmap? PatchE { get; set; }  // middle
        public SKBitmap? PatchF { get; set; }  // right side
        public SKBitmap? PatchG { get; set; }  // bottom left corner
        public SKBitmap? PatchH { get; set; }  // bottom middle
        public SKBitmap? PatchI { get; set; }  // bottom right corner
        public bool FrameEnabled { get; set; } = true;
        public Color FrameTint { get; set; } = Color.WhiteSmoke;
        public float FrameScale { get; set; } = 1.0F;
        public SKPaint? FramePaint { get; set; }
        public float FrameCenterLeft { get; set; }
        public float FrameCenterTop { get; set; }
        public float FrameCenterRight { get; set; }
        public float FrameCenterBottom { get; set; }

        public override void Render(SKCanvas canvas)
        {
            if (!FrameEnabled)
            {
                return;
            }

            if (FrameBitmap != null
            && PatchA != null
            && PatchB != null
            && PatchC != null
            && PatchD != null
            && PatchF != null
            && PatchG != null
            && PatchH != null
            && PatchI != null)
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
                canvas.DrawBitmap(PatchA, 0, 0, FramePaint);

                // tile Patch_B - has to be an integral number of tiles;
                float patch_b_tile_length = Width - PatchA.Width - PatchC.Width;

                int num_patch_b_tiles = Math.Max((int)Math.Floor(patch_b_tile_length / PatchB.Width), 1);

                int newWidth = (int)Math.Ceiling(patch_b_tile_length / num_patch_b_tiles);

                num_patch_b_tiles = (int)Math.Round((float)patch_b_tile_length / newWidth);

                while ((newWidth * num_patch_b_tiles) + PatchA.Width + PatchC.Width < Width)
                {
                    newWidth++;
                }

                // scale Patch_B so that it tiles an integral number of times
                using SKBitmap scaled_B = new(newWidth, PatchB.Height);
                PatchB.ScalePixels(scaled_B, SKSamplingOptions.Default);

                int left = PatchA.Width;

                for (int i = 0; i < num_patch_b_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_B, left, 0, FramePaint);
                    left += scaled_B.Width;
                }
                canvas.DrawBitmap(PatchC, Width - PatchC.Width, 0, FramePaint);

                // tile Patch_D - has to be an integral number of tiles;
                float patch_d_tile_height = Height - PatchA.Height - PatchG.Height;

                int num_patch_d_tiles = Math.Max((int)(patch_d_tile_height / PatchD.Height), 1);

                int newHeight = (int)Math.Ceiling(patch_d_tile_height / num_patch_d_tiles);

                num_patch_d_tiles = (int)Math.Round((float)patch_d_tile_height / newHeight);

                while ((newHeight * num_patch_d_tiles) + PatchA.Height + PatchG.Height < Height)
                {
                    newHeight++;
                }

                // scale Patch_D so that it tiles an integral number of times
                // scaled D patch has same width and new height
                using SKBitmap scaled_D = new(PatchD.Width, newHeight);
                PatchD.ScalePixels(scaled_D, SKSamplingOptions.Default);

                int top = PatchA.Height;

                for (int i = 0; i < num_patch_d_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_D, 0, top, FramePaint);
                    top += scaled_D.Height;
                }

                float patch_f_tile_height = Height - PatchC.Height - PatchI.Height;

                int num_patch_f_tiles = Math.Max((int)(patch_f_tile_height / PatchF.Height), 1);

                int newFHeight = (int)Math.Ceiling(patch_f_tile_height / num_patch_f_tiles);

                num_patch_f_tiles = (int)Math.Round((float)patch_f_tile_height / newHeight);

                while ((newFHeight * num_patch_f_tiles) + PatchC.Height + PatchI.Height < Height)
                {
                    newFHeight++;
                }

                // scale Patch_F so that it tiles an integral number of times
                using SKBitmap scaled_F = new(PatchF.Width, newFHeight);
                PatchF.ScalePixels(scaled_F, SKSamplingOptions.Default);

                top = PatchC.Height;

                for (int i = 0; i < num_patch_f_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_F, Width - scaled_F.Width, top, FramePaint);
                    top += scaled_F.Height;
                }

                // frame bottom
                canvas.DrawBitmap(PatchG, 0, Height - PatchG.Height, FramePaint);

                // scale Patch_H so that it tiles an integral number of times
                float patch_h_tile_length = Width - PatchG.Width - PatchI.Width;

                int num_patch_h_tiles = Math.Max((int)Math.Floor(patch_h_tile_length / PatchH.Width), 1);

                int newHWidth = (int)Math.Ceiling(patch_h_tile_length / num_patch_h_tiles);

                num_patch_h_tiles = (int)Math.Round((float)patch_h_tile_length / newWidth);

                while ((newHWidth * num_patch_h_tiles) + PatchG.Width + PatchI.Width < Width)
                {
                    newHWidth++;
                }

                using SKBitmap scaled_H = new(newHWidth, PatchH.Height);
                PatchH.ScalePixels(scaled_H, SKSamplingOptions.Default);

                left = PatchG.Width;

                // patch H is tiled the same number of times as patch B
                for (int i = 0; i < num_patch_h_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_H, left, Height - PatchH.Height, FramePaint);
                    left += scaled_H.Width;
                }

                canvas.DrawBitmap(PatchI, Width - PatchI.Width, Height - PatchI.Height, FramePaint);
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
#pragma warning disable CS8601 // Possible null reference assignment

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
                using MemoryStream ms = new(imageBytes);
                FrameBitmap = SKBitmap.Decode(ms);
            }

            IEnumerable<XElement> frameTintElem = mapFrameDoc.Descendants(ns + "FrameTint");
            if (frameTintElem != null && frameTintElem.Any() && frameTintElem.First() != null)
            {
                string? frameTint = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameTint").Value).FirstOrDefault();

                int argbValue = Color.White.ToArgb();

                if (frameTint.StartsWith('#'))
                {
                    argbValue = ColorTranslator.FromHtml(frameTint).ToArgb();
                }
                else if (int.TryParse(frameTint, out int n))
                {
                    argbValue = n;
                }

                FrameTint = Color.FromArgb(argbValue);
            }
            else
            {
                FrameTint = Color.White;
            }

            IEnumerable<XElement?> frameScaleElem = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameScale"));
            if (frameScaleElem.First() != null)
            {
                string? frameScale = mapFrameDoc.Descendants().Select(x => x.Element(ns + "FrameScale").Value).FirstOrDefault();

                if (float.TryParse(frameScale, out float n))
                {
                    FrameScale = n;

                    if (FrameScale > 1.0F)
                    {
                        FrameScale /= 100.0F;
                    }
                }
                else
                {
                    FrameScale = 1.0F;
                }
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

            //FrameManager.CompletePlacedFrame(this);

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            if (FrameBitmap == null) return;

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

            writer.WriteStartElement("FrameTint");
            writer.WriteValue(FrameTint.ToArgb());
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
