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

namespace RealmStudio
{
    public class DrawnRegularPolygon : DrawnMapComponent, IXmlSerializable
    {
        private SKPoint _topLeft;
        private SKPoint _bottomRight;
        private int _sides = 5; // Default to pentagon
        private SKColor _color = SKColors.Black;
        private SKColor _fillColor = SKColors.Transparent;
        private int _brushSize = 2;
        private int _rotation;
        private DrawingFillType _fillType = DrawingFillType.None;
        private Bitmap? _fillBitmap;
        private SKShader? _shader;

        public SKPoint TopLeft
        {
            get => _topLeft;
            set => _topLeft = value;
        }
        public SKPoint BottomRight
        {
            get => _bottomRight;
            set => _bottomRight = value;
        }

        public SKColor Color
        {
            get => _color;
            set => _color = value;
        }

        public SKColor FillColor
        {
            get => _fillColor;
            set => _fillColor = value;
        }

        public int BrushSize
        {
            get => _brushSize;
            set => _brushSize = value;
        }
        public int Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }
        public DrawingFillType FillType
        {
            get => _fillType;
            set => _fillType = value;
        }

        public int Sides
        {
            get => _sides;
            set
            {
                if (value < 3)
                    throw new ArgumentOutOfRangeException(nameof(Sides), "A polygon must have at least 3 sides.");
                _sides = value;
            }
        }

        public Bitmap? FillBitmap
        {
            get => _fillBitmap;
            set => _fillBitmap = value;
        }

        public SKShader? Shader
        {
            get => _shader;
            set => _shader = value;
        }

        public override void Render(SKCanvas canvas)
        {
            using SKPaint paint = new()
            {
                Style = SKPaintStyle.Stroke,
                Color = Color,
                StrokeWidth = BrushSize,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Butt
            };

            using SKPaint fillPaint = new()
            {
                Style = SKPaintStyle.Fill,
                Color = FillColor,
                IsAntialias = true
            };

            if (FillType == DrawingFillType.Texture && Shader != null)
            {
                fillPaint.Shader = Shader;
                fillPaint.Style = SKPaintStyle.StrokeAndFill;
            }
            else if (FillType == DrawingFillType.Color)
            {
                fillPaint.Color = FillColor;
                fillPaint.Style = SKPaintStyle.StrokeAndFill;
            }
            else
            {
                fillPaint.Style = SKPaintStyle.Stroke;
            }

            SKRect rect = new(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
            SKPoint centerPoint = rect.Location;

            List<SKPoint> points = DrawingMethods.PolyPoints(TopLeft, Sides, rect.Width, 3.0F * (float)Math.PI / 2.0F);

            SKPath path = new();

            path.MoveTo(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i]);
            }

            path.Close();

            Bounds = path.Bounds;
            Bounds = SKRect.Inflate(Bounds, 2, 2);

            using SKAutoCanvasRestore autoRestore = new(canvas, true);
            if (Rotation != 0)
            {
                canvas.RotateDegrees(Rotation, Bounds.MidX, Bounds.MidY);
            }

            base.Render(canvas);

            canvas.DrawPath(path, fillPaint);
            canvas.DrawPath(path, paint);
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

            XDocument polyDoc = XDocument.Parse(content);

            if (polyDoc.Root == null)
            {
                throw new InvalidDataException("The polygon XML data is missing the root element.");
            }

            XAttribute? xAttr = polyDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = polyDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? wAttr = polyDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = (int)float.Parse(wAttr.Value);
            }

            XAttribute? hAttr = polyDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = (int)float.Parse(hAttr.Value);
            }

            XElement? topLeftElement = polyDoc.Root.Element(ns + "TopLeft");
            if (topLeftElement != null)
            {
                XAttribute? tlxAttr = topLeftElement.Attribute("X");
                XAttribute? tlyAttr = topLeftElement.Attribute("Y");
                if (tlxAttr != null && tlyAttr != null)
                {
                    _topLeft = new SKPoint((int)float.Parse(tlxAttr.Value), (int)float.Parse(tlyAttr.Value));
                }
            }

            XElement? bottomRightElement = polyDoc.Root.Element(ns + "BottomRight");
            if (bottomRightElement != null)
            {
                XAttribute? brxAttr = bottomRightElement.Attribute("X");
                XAttribute? bryAttr = bottomRightElement.Attribute("Y");
                if (brxAttr != null && bryAttr != null)
                {
                    _bottomRight = new SKPoint((int)float.Parse(brxAttr.Value), (int)float.Parse(bryAttr.Value));
                }
            }

            SKRect rect = new(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
            Bounds = rect;

            XElement? colorElement = polyDoc.Root.Element(ns + "Color");
            if (colorElement != null)
            {
                _color = SKColor.Parse(colorElement.Value);
            }

            XElement? brushSizeElement = polyDoc.Root.Element(ns + "BrushSize");
            if (brushSizeElement != null)
            {
                _brushSize = (int)float.Parse(brushSizeElement.Value);
            }

            XElement? rotationElement = polyDoc.Root.Element(ns + "Rotation");
            if (rotationElement != null)
            {
                _rotation = (int)float.Parse(rotationElement.Value);
            }

            XElement? fillTypeElement = polyDoc.Root.Element(ns + "FillType");
            if (fillTypeElement != null)
            {
                _fillType = Enum.Parse<DrawingFillType>(fillTypeElement.Value);
            }

            XElement? sidesElement = polyDoc.Root.Element(ns + "Sides");
            if (sidesElement != null)
            {
                Sides = (int)float.Parse(sidesElement.Value);
            }

            XElement? fillBitmapElement = polyDoc.Root.Element(ns + "FillBitmap");
            if (fillBitmapElement != null)
            {
                string base64Data = fillBitmapElement.Value;
                byte[] fillBitmapData = Convert.FromBase64String(base64Data);
                using MemoryStream ms = new(fillBitmapData);
                SKBitmap skBitmap = SKBitmap.Decode(ms);
                _fillBitmap = skBitmap.ToBitmap();

                SKShader fillShader = SKShader.CreateBitmap(skBitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                Shader = fillShader;
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

            writer.WriteStartElement("BottomRight");
            writer.WriteAttributeString("X", BottomRight.X.ToString());
            writer.WriteAttributeString("Y", BottomRight.Y.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("Color", Color.ToString());
            writer.WriteElementString("BrushSize", BrushSize.ToString());
            writer.WriteElementString("Rotation", Rotation.ToString());
            writer.WriteElementString("FillType", FillType.ToString());
            writer.WriteElementString("Sides", Sides.ToString());


            if (FillType == DrawingFillType.Texture && FillBitmap != null)
            {
                using MemoryStream ms = new();
                using SKManagedWStream wstream = new(ms);
                FillBitmap.ToSKBitmap().Encode(wstream, SKEncodedImageFormat.Png, 100);
                byte[] fillBitmapData = ms.ToArray();
                writer.WriteStartElement("FillBitmap");
                writer.WriteBase64(fillBitmapData, 0, fillBitmapData.Length);
                writer.WriteEndElement();
            }
        }
    }
}