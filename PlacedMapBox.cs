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
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class PlacedMapBox : MapComponent, IXmlSerializable
    {
        public Guid BoxGuid = Guid.NewGuid();

        public SKBitmap? BoxBitmap { get; set; } = null;

        public Color BoxTint { get; set; } = Color.White;

        public SKPaint? BoxPaint { get; set; } = null;

        public bool IsSelected { get; set; } = false;

        public float BoxCenterLeft { get; set; } = 0;
        public float BoxCenterTop { get; set; } = 0;
        public float BoxCenterRight { get; set; } = 0;
        public float BoxCenterBottom { get; set; } = 0;

        public SKBitmap Patch_A { get; set; } = new();  // top left corner
        public SKBitmap Patch_B { get; set; } = new(); // top middle
        public SKBitmap Patch_C { get; set; } = new(); // top right corner
        public SKBitmap Patch_D { get; set; } = new(); // left side
        public SKBitmap Patch_E { get; set; } = new(); // middle
        public SKBitmap Patch_F { get; set; } = new(); // right side
        public SKBitmap Patch_G { get; set; } = new(); // bottom left corner
        public SKBitmap Patch_H { get; set; } = new(); // bottom middle
        public SKBitmap Patch_I { get; set; } = new(); // bottom right corner

        public void SetBoxBitmap(SKBitmap b)
        {
            BoxBitmap = b;
        }

        public override void Render(SKCanvas canvas)
        {
            if (BoxBitmap != null
                && (!Patch_A.IsNull && Patch_A.Width > 0 && Patch_A.Height > 0)
                && (!Patch_B.IsNull && Patch_B.Width > 0 && Patch_B.Height > 0)
                && (!Patch_C.IsNull && Patch_C.Width > 0 && Patch_C.Height > 0)
                && (!Patch_D.IsNull && Patch_D.Width > 0 && Patch_D.Height > 0)
                && (!Patch_E.IsNull && Patch_E.Width > 0 && Patch_E.Height > 0)
                && (!Patch_F.IsNull && Patch_F.Width > 0 && Patch_F.Height > 0)
                && (!Patch_G.IsNull && Patch_G.Width > 0 && Patch_G.Height > 0)
                && (!Patch_H.IsNull && Patch_H.Width > 0 && Patch_H.Height > 0)
                && (!Patch_I.IsNull && Patch_I.Width > 0 && Patch_I.Height > 0))
            {
                // frame top
                canvas.DrawBitmap(Patch_A, X, Y, BoxPaint);

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
                Patch_B.ScalePixels(scaled_B, SKSamplingOptions.Default);

                int left = X + Patch_A.Width;

                for (int i = 0; i < num_patch_b_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_B, left, Y, BoxPaint);
                    left += scaled_B.Width;
                }

                canvas.DrawBitmap(Patch_C, X + Width - Patch_C.Width, Y, BoxPaint);

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
                Patch_D.ScalePixels(scaled_D, SKSamplingOptions.Default);

                int top = Y + Patch_A.Height;

                for (int i = 0; i < num_patch_d_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_D, X, top, BoxPaint);
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
                Patch_F.ScalePixels(scaled_F, SKSamplingOptions.Default);

                top = Y + Patch_C.Height;

                for (int i = 0; i < num_patch_f_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_F, X + Width - scaled_F.Width, top, BoxPaint);
                    top += scaled_F.Height;
                }

                // frame bottom
                canvas.DrawBitmap(Patch_G, X, Y + Height - Patch_G.Height, BoxPaint);

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
                Patch_H.ScalePixels(scaled_H, SKSamplingOptions.Default);

                left = X + Patch_G.Width;

                // patch H is tiled the same number of times as patch B
                for (int i = 0; i < num_patch_h_tiles; i++)
                {
                    canvas.DrawBitmap(scaled_H, left, Y + Height - Patch_H.Height, BoxPaint);
                    left += scaled_H.Width;
                }

                canvas.DrawBitmap(Patch_I, X + Width - Patch_I.Width, Y + Height - Patch_I.Height, BoxPaint);

                // box center
                canvas.DrawBitmap(Patch_E, X + Patch_D.Width, Y + Patch_B.Height, BoxPaint);

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
#pragma warning disable CS8601 // Possible null reference assignment.

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
                using (MemoryStream ms = new(imageBytes))
                {
                    BoxBitmap = SKBitmap.Decode(ms);
                }
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
            if (bclElem != null && bclElem.Count() > 0 && bclElem.First() != null)
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
            if (bctElem != null && bctElem.Count() > 0 && bctElem.First() != null)
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
            if (bcrElem != null && bcrElem.Count() > 0 && bctElem.First() != null)
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
            if (bcbElem != null && bcbElem.Count() > 0 && bctElem.First() != null)
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
