/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudioX
{
    public sealed class DrawnStamp : DrawnMapComponent, IXmlSerializable
    {
        private SKPoint _topLeft;
        private int _rotation;
        private float _opacity = 1.0f;
        private float _scale = 1.0f;
        private SKBitmap? _stampBitmap;

        public SKPoint TopLeft
        {
            get => _topLeft;
            set => _topLeft = value;
        }

        public int Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public float Opacity
        {
            get => _opacity;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Opacity must be between 0 and 1.");
                }
                _opacity = value;
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Scale must be greater than 0.");
                }
                _scale = value;
            }
        }

        public SKBitmap? StampBitmap
        {
            get => _stampBitmap;
            set
            {
                _stampBitmap = value ?? throw new ArgumentNullException(nameof(value), "Stamp bitmap cannot be null.");
            }
        }

        public override void Render(SKCanvas canvas)
        {
            if (StampBitmap != null)
            {
                using Bitmap stampBitmap = DrawingMethods.SetBitmapOpacity(StampBitmap.ToBitmap(), Opacity);

                using Bitmap scaledStamp = DrawingMethods.ScaleBitmap(stampBitmap, (int)(stampBitmap.Width * Scale), (int)(stampBitmap.Height * Scale));

                using SKBitmap rotatedAndScaledStamp = DrawingMethods.RotateSKBitmap(scaledStamp.ToSKBitmap(), Rotation, false);

                canvas.DrawBitmap(rotatedAndScaledStamp,
                    new SKPoint(TopLeft.X - (rotatedAndScaledStamp.Width / 2), TopLeft.Y - (rotatedAndScaledStamp.Height / 2)), null);

                Bounds = new SKRect(TopLeft.X - (rotatedAndScaledStamp.Width / 2), TopLeft.Y - (rotatedAndScaledStamp.Height / 2),
                                    TopLeft.X + (rotatedAndScaledStamp.Width / 2), TopLeft.Y + (rotatedAndScaledStamp.Height / 2));

                base.Render(canvas);
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XNamespace ns = "RealmStudio";
            string content = reader.ReadOuterXml();

            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            XDocument stampDoc = XDocument.Parse(content);

            if (stampDoc.Root == null)
            {
                throw new InvalidDataException("The stamp XML data is missing the root element.");
            }

            XAttribute? xAttr = stampDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = stampDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? wAttr = stampDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = (int)float.Parse(wAttr.Value);
            }

            XAttribute? hAttr = stampDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = (int)float.Parse(hAttr.Value);
            }

            XElement? topLeftElement = stampDoc.Root.Element(ns + "TopLeft");
            if (topLeftElement != null)
            {
                XAttribute? tlxAttr = topLeftElement.Attribute("X");
                XAttribute? tlyAttr = topLeftElement.Attribute("Y");
                if (tlxAttr != null && tlyAttr != null)
                {
                    _topLeft = new SKPoint((int)float.Parse(tlxAttr.Value), (int)float.Parse(tlyAttr.Value));
                }
            }

            XElement? scaleElement = stampDoc.Root.Element(ns + "Scale");
            if (scaleElement != null)
            {
                _scale = float.Parse(scaleElement.Value);
            }

            XElement? opacityElement = stampDoc.Root.Element(ns + "Opacity");
            if (opacityElement != null)
            {
                _opacity = float.Parse(opacityElement.Value);
            }

            XElement? rotationElement = stampDoc.Root.Element(ns + "Rotation");
            if (rotationElement != null)
            {
                _rotation = (int)float.Parse(rotationElement.Value);
            }

            XElement? stampBitmapElement = stampDoc.Root.Element(ns + "StampBitmap");
            if (stampBitmapElement != null)
            {
                string base64Data = stampBitmapElement.Value;
                byte[] stampBitmapData = Convert.FromBase64String(base64Data);
                using MemoryStream ms = new(stampBitmapData);
                SKBitmap skBitmap = SKBitmap.Decode(ms);
                _stampBitmap = skBitmap.Copy();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            writer.WriteStartElement("TopLeft");
            writer.WriteAttributeString("X", TopLeft.X.ToString());
            writer.WriteAttributeString("Y", TopLeft.Y.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("Rotation", Rotation.ToString());
            writer.WriteElementString("Opacity", Opacity.ToString());
            writer.WriteElementString("Scale", Scale.ToString());

            if (StampBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);
                StampBitmap.Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] stampBitmapData = ms.ToArray();
                writer.WriteStartElement("StampBitmap");
                writer.WriteBase64(stampBitmapData, 0, stampBitmapData.Length);
                writer.WriteEndElement();
            }
        }
    }
}
