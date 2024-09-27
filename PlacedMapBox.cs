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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    internal class PlacedMapBox : MapComponent, IXmlSerializable
    {
        public Guid BoxGuid = Guid.NewGuid();

        public Bitmap? BoxBitmap { get; set; } = null;

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

        public void SetBoxBitmap(Bitmap b)
        {
            BoxBitmap = b;
        }

        public override void Render(SKCanvas canvas)
        {
            if (BoxBitmap != null
                && Patch_A != null
                && Patch_B != null
                && Patch_C != null
                && Patch_D != null
                && Patch_E != null
                && Patch_F != null
                && Patch_G != null
                && Patch_H != null
                && Patch_I != null)
            {
                using SKPaint boxPaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    ColorFilter = SKColorFilter.CreateBlendMode(
                        Extensions.ToSKColor(BoxTint),
                        SKBlendMode.Modulate) // combine the tint with the bitmap color
                };

                BoxPaint = boxPaint.Clone();

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
                Patch_B.ScalePixels(scaled_B, SKFilterQuality.High);

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
                Patch_D.ScalePixels(scaled_D, SKFilterQuality.High);

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
                Patch_F.ScalePixels(scaled_F, SKFilterQuality.High);

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
                Patch_H.ScalePixels(scaled_H, SKFilterQuality.High);

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
                    BoxBitmap = Extensions.ToBitmap(SKBitmap.Decode(ms));
                }
            }

            IEnumerable<XElement> boxTintElem = mapBoxDoc.Descendants(ns + "BoxTint");
            if (boxTintElem.First() != null)
            {
                string? boxTint = mapBoxDoc.Descendants().Select(x => x.Element(ns + "BoxTint").Value).FirstOrDefault();
                BoxTint = ColorTranslator.FromHtml(boxTint);
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

            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);
            Extensions.ToSKBitmap(BoxBitmap).Encode(wstream, SKEncodedImageFormat.Png, 100);
            byte[] bitmapData = ms.ToArray();
            writer.WriteStartElement("BoxBitmap");
            writer.WriteBase64(bitmapData, 0, bitmapData.Length);
            writer.WriteEndElement();

            XmlColor boxTint = new(BoxTint);
            writer.WriteStartElement("BoxTint");
            boxTint.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
