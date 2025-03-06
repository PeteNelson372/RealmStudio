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
using SkiaSharp.Views.Desktop;
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class PlacedMapBox : MapComponent, IXmlSerializable
    {
        public Guid BoxGuid { get; set; } = Guid.NewGuid();

        public SKBitmap? BoxBitmap { get; set; }

        public Color BoxTint { get; set; } = Color.White;

        public SKPaint? BoxPaint { get; set; }

        public bool IsSelected { get; set; }

        public float BoxCenterLeft { get; set; }
        public float BoxCenterTop { get; set; }
        public float BoxCenterRight { get; set; }
        public float BoxCenterBottom { get; set; }

        public SKBitmap PatchA { get; set; } = new();  // top left corner
        public SKBitmap PatchB { get; set; } = new(); // top middle
        public SKBitmap PatchC { get; set; } = new(); // top right corner
        public SKBitmap PatchD { get; set; } = new(); // left side
        public SKBitmap PatchE { get; set; } = new(); // middle
        public SKBitmap PatchF { get; set; } = new(); // right side
        public SKBitmap PatchG { get; set; } = new(); // bottom left corner
        public SKBitmap PatchH { get; set; } = new(); // bottom middle
        public SKBitmap PatchI { get; set; } = new(); // bottom right corner

        public PlacedMapBox() { }

        public PlacedMapBox(PlacedMapBox box)
        {
            X = box.X;
            Y = box.Y;
            Width = box.Width;
            Height = box.Height;
            BoxBitmap = box.BoxBitmap?.Copy();
            BoxTint = box.BoxTint;
            BoxPaint = box.BoxPaint?.Clone();
            BoxCenterLeft = box.BoxCenterLeft;
            BoxCenterTop = box.BoxCenterTop;
            BoxCenterRight = box.BoxCenterRight;
            BoxCenterBottom = box.BoxCenterBottom;
        }

        public void SetBoxBitmap(SKBitmap b)
        {
            BoxBitmap = b;
        }

        public override void Render(SKCanvas canvas)
        {
            if (BoxBitmap != null
                && (!PatchA.IsNull && PatchA.Width > 0 && PatchA.Height > 0)
                && (!PatchB.IsNull && PatchB.Width > 0 && PatchB.Height > 0)
                && (!PatchC.IsNull && PatchC.Width > 0 && PatchC.Height > 0)
                && (!PatchD.IsNull && PatchD.Width > 0 && PatchD.Height > 0)
                && (!PatchE.IsNull && PatchE.Width > 0 && PatchE.Height > 0)
                && (!PatchF.IsNull && PatchF.Width > 0 && PatchF.Height > 0)
                && (!PatchG.IsNull && PatchG.Width > 0 && PatchG.Height > 0)
                && (!PatchH.IsNull && PatchH.Width > 0 && PatchH.Height > 0)
                && (!PatchI.IsNull && PatchI.Width > 0 && PatchI.Height > 0))
            {
                // frame top
                canvas.DrawBitmap(PatchA, X, Y, BoxPaint);

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

                int left = X + PatchA.Width;

                for (int i = 0; i < num_patch_b_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_B, left, Y, BoxPaint);
                    left += scaled_B.Width;
                }

                canvas.DrawBitmap(PatchC, X + Width - PatchC.Width, Y, BoxPaint);

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

                int top = Y + PatchA.Height;

                for (int i = 0; i < num_patch_d_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_D, X, top, BoxPaint);
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

                top = Y + PatchC.Height;

                for (int i = 0; i < num_patch_f_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_F, X + Width - scaled_F.Width, top, BoxPaint);
                    top += scaled_F.Height;
                }

                // frame bottom
                canvas.DrawBitmap(PatchG, X, Y + Height - PatchG.Height, BoxPaint);

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

                left = X + PatchG.Width;

                // patch H is tiled the same number of times as patch B
                for (int i = 0; i < num_patch_h_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_H, left, Y + Height - PatchH.Height, BoxPaint);
                    left += scaled_H.Width;
                }

                canvas.DrawBitmap(PatchI, X + Width - PatchI.Width, Y + Height - PatchI.Height, BoxPaint);

                // box center
                canvas.DrawBitmap(PatchE, X + PatchD.Width, Y + PatchB.Height, BoxPaint);

                if (IsSelected)
                {
                    canvas.DrawRect(X, Y, Width, Height, PaintObjects.BoxSelectPaint);
                }

            }
        }

        private void GetBoxCenterFromMapBox()
        {
            if (BoxBitmap == null) { return; }

            byte[] bbBytes;

            using (var mstream = new MemoryStream())
            {
                Bitmap b = BoxBitmap.ToBitmap();
                b.Save(mstream, ImageFormat.Bmp);
                bbBytes = mstream.ToArray();
            }

            string bb64 = Convert.ToBase64String(bbBytes);

            for (int i = 0; i < AssetManager.MAP_BOX_LIST.Count; i++)
            {
                if (AssetManager.MAP_BOX_LIST[i].BoxBitmap == null)
                {
                    continue;
                }
                else
                {
                    byte[] mblBytes;

                    using (var mstream2 = new MemoryStream())
                    {
                        AssetManager.MAP_BOX_LIST[i].BoxBitmap?.Save(mstream2, ImageFormat.Bmp);
                        mblBytes = mstream2.ToArray();
                    }

                    string mblb64 = Convert.ToBase64String(bbBytes);

                    if (mblb64.CompareTo(bb64) == 0)
                    {
                        BoxCenterLeft = AssetManager.MAP_BOX_LIST[i].BoxCenterLeft;
                        BoxCenterTop = AssetManager.MAP_BOX_LIST[i].BoxCenterTop;
                        BoxCenterRight = AssetManager.MAP_BOX_LIST[i].BoxCenterRight;
                        BoxCenterBottom = AssetManager.MAP_BOX_LIST[i].BoxCenterBottom;
                        break;
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

            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();
            XDocument mapBoxDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapBoxDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapBoxDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapBoxDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapBoxDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> guidElemEnum = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxGuid"));
            if (guidElemEnum.First() != null)
            {
                string? boxGuid = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxGuid").Value).FirstOrDefault();
                BoxGuid = Guid.Parse(boxGuid);
            }

            IEnumerable<XElement?> boxBitmapEnum = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxBitmap"));
            if (boxBitmapEnum.First() != null)
            {
                string? boxBitmapBase64String = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxBitmap").Value).FirstOrDefault();

                byte[] imageBytes = Convert.FromBase64String(boxBitmapBase64String);

                // Create an image from the byte array
                using MemoryStream ms = new(imageBytes);
                BoxBitmap = SKBitmap.Decode(ms);
            }

            IEnumerable<XElement> boxTintElem = mapBoxDoc.Descendants(ns + "BoxTint");
            if (boxTintElem != null && boxTintElem.Any() && boxTintElem.First() != null)
            {
                string? boxTint = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxTint").Value).FirstOrDefault();

                int argbValue = 0;

                if (boxTint.StartsWith('#'))
                {
                    argbValue = ColorTranslator.FromHtml(boxTint).ToArgb();
                }

                if (int.TryParse(boxTint, out int n))
                {
                    //if (n > 0)
                    {
                        argbValue = n;
                    }
                    //else
                    //{
                    //    argbValue = Color.FromArgb(126, 0, 0, 0).ToArgb();
                    //}
                }

                BoxTint = Color.FromArgb(argbValue);
            }
            else
            {
                BoxTint = Color.White;
            }

            IEnumerable<XElement?> bclElem = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterLeft"));
            if (bclElem != null && bclElem.Any() && bclElem.First() != null)
            {
                string? centerLeft = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterLeft").Value).FirstOrDefault();

                if (float.TryParse(centerLeft, out float n))
                {
                    BoxCenterLeft = n;
                }
                else
                {
                    BoxCenterLeft = 0;
                }
            }
            else
            {
                BoxCenterLeft = 0;
            }

            IEnumerable<XElement?> bctElem = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterTop"));
            if (bctElem != null && bctElem.Any() && bctElem.First() != null)
            {
                string? centerTop = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterTop").Value).FirstOrDefault();

                if (float.TryParse(centerTop, out float n))
                {
                    BoxCenterTop = n;
                }
                else
                {
                    BoxCenterTop = 0;
                }
            }
            else
            {
                BoxCenterTop = 0;
            }

            IEnumerable<XElement?> bcrElem = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterRight"));
            if (bcrElem != null && bcrElem.Any() && bctElem.First() != null)
            {
                string? centerRight = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterRight").Value).FirstOrDefault();

                if (float.TryParse(centerRight, out float n))
                {
                    BoxCenterRight = n;
                }
                else
                {
                    BoxCenterRight = 0;
                }
            }
            else
            {
                BoxCenterRight = 0;
            }

            IEnumerable<XElement?> bcbElem = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterBottom"));
            if (bcbElem != null && bcbElem.Any() && bctElem.First() != null)
            {
                string? centerBottom = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxCenterBottom").Value).FirstOrDefault();

                if (float.TryParse(centerBottom, out float n))
                {
                    BoxCenterBottom = n;
                }
                else
                {
                    BoxCenterBottom = 0;
                }
            }
            else
            {
                BoxCenterBottom = 0;
            }


            // if box center data could not be retrieved from the saved map XML file, attempt to get it from
            // the map box data kept by the AssetManager
            if (BoxCenterLeft == 0 || BoxCenterTop == 0 || BoxCenterRight == 0 || BoxCenterBottom == 0)
            {
                GetBoxCenterFromMapBox();
            }

#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            writer.WriteStartElement("BoxGuid");
            writer.WriteString(BoxGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("BoxCenterLeft");
            writer.WriteValue(BoxCenterLeft);
            writer.WriteEndElement();

            writer.WriteStartElement("BoxCenterTop");
            writer.WriteValue(BoxCenterTop);
            writer.WriteEndElement();

            writer.WriteStartElement("BoxCenterRight");
            writer.WriteValue(BoxCenterRight);
            writer.WriteEndElement();

            writer.WriteStartElement("BoxCenterBottom");
            writer.WriteValue(BoxCenterBottom);
            writer.WriteEndElement();

            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);
            BoxBitmap?.Encode(wstream, SKEncodedImageFormat.Png, 100);
            byte[] bitmapData = ms.ToArray();
            writer.WriteStartElement("BoxBitmap");
            writer.WriteBase64(bitmapData, 0, bitmapData.Length);
            writer.WriteEndElement();

            int boxTint = BoxTint.ToArgb();
            writer.WriteStartElement("BoxTint");
            writer.WriteValue(boxTint);
            writer.WriteEndElement();
        }
    }
}
