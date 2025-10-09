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
using System.IO;
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

        public float BoxCenterLeft { get; set; }
        public float BoxCenterTop { get; set; }
        public float BoxCenterRight { get; set; }
        public float BoxCenterBottom { get; set; }

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
            try
            {
                // the box center can be outside the bounds of the bitmap if
                // the box is drawn to be very narrow in height or width
                if (BoxBitmap != null)
                {
                    canvas.DrawBitmapNinePatch(BoxBitmap,
                        new SKRectI((int)BoxCenterLeft, (int)BoxCenterTop, (int)BoxCenterRight, (int)BoxCenterBottom),
                        new SKRect(X, Y, X + Width, Y + Height),
                        BoxPaint);

                    if (IsSelected)
                    {
                        canvas.DrawRect(X, Y, Width, Height, PaintObjects.BoxSelectPaint);
                    }
                }
            }
            catch { }
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

                int argbValue = Color.White.ToArgb();

                if (boxTint.StartsWith('#'))
                {
                    argbValue = ColorTranslator.FromHtml(boxTint).ToArgb();
                }
                else if (int.TryParse(boxTint, out int n))
                {
                    argbValue = n;
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
