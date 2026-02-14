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
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public class DrawnTriangle : DrawnMapComponent, IXmlSerializable
    {
        private SKPoint _topLeft;
        private SKPoint _bottomRight;
        private SKColor _color = SKColors.Black;
        private SKColor _fillColor = SKColors.Transparent;
        private int _brushSize = 2;
        private int _rotation;
        private DrawingFillType _fillType = DrawingFillType.None;
        private SKShader? _shader;
        private Bitmap? _fillBitmap;
        private bool _drawRight;

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

        public bool DrawRight
        {
            get => _drawRight;
            set => _drawRight = value;
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

            SKPath path = new();
            SKPoint triangleTop = new(_topLeft.X + (_bottomRight.X - _topLeft.X) / 2, _topLeft.Y);
            SKPoint triangleBottomLeft = new(_topLeft.X, _bottomRight.Y);
            SKPoint triangleBottomRight = new(_bottomRight.X, _bottomRight.Y);

            if (DrawRight)
            {
                triangleTop = new(_topLeft.X, _topLeft.Y);
                triangleBottomLeft = new(_topLeft.X, _bottomRight.Y);
                triangleBottomRight = new(_bottomRight.X, _bottomRight.Y);
            }

            path.MoveTo(triangleTop);
            path.LineTo(triangleBottomLeft);
            path.LineTo(triangleBottomRight);
            path.Close();

            Bounds = path.Bounds;

            using SKAutoCanvasRestore autoRestore = new(canvas, true);
            if (Rotation != 0)
            {
                canvas.RotateDegrees(Rotation, (_topLeft.X + _bottomRight.X) / 2, (_topLeft.Y + _bottomRight.Y) / 2);
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

            XDocument ellipseDoc = XDocument.Parse(content);

            if (ellipseDoc.Root == null)
            {
                throw new InvalidDataException("The diamond XML data is missing the root element.");
            }

            XAttribute? xAttr = ellipseDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = ellipseDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? wAttr = ellipseDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = (int)float.Parse(wAttr.Value);
            }

            XAttribute? hAttr = ellipseDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = (int)float.Parse(hAttr.Value);
            }

            XElement? topLeftElement = ellipseDoc.Root.Element(ns + "TopLeft");
            if (topLeftElement != null)
            {
                XAttribute? tlxAttr = topLeftElement.Attribute("X");
                XAttribute? tlyAttr = topLeftElement.Attribute("Y");
                if (tlxAttr != null && tlyAttr != null)
                {
                    _topLeft = new SKPoint((int)float.Parse(tlxAttr.Value), (int)float.Parse(tlyAttr.Value));
                }
            }

            XElement? bottomRightElement = ellipseDoc.Root.Element(ns + "BottomRight");
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

            XElement? colorElement = ellipseDoc.Root.Element(ns + "Color");
            if (colorElement != null)
            {
                _color = SKColor.Parse(colorElement.Value);
            }

            XElement? brushSizeElement = ellipseDoc.Root.Element(ns + "BrushSize");
            if (brushSizeElement != null)
            {
                _brushSize = (int)float.Parse(brushSizeElement.Value);
            }

            XElement? rotationElement = ellipseDoc.Root.Element(ns + "Rotation");
            if (rotationElement != null)
            {
                _rotation = (int)float.Parse(rotationElement.Value);
            }

            XElement? drawRightElement = ellipseDoc.Root.Element(ns + "DrawRight");
            if (drawRightElement != null)
            {
                _drawRight = bool.Parse(drawRightElement.Value);
            }

            XElement? fillTypeElement = ellipseDoc.Root.Element(ns + "FillType");
            if (fillTypeElement != null)
            {
                _fillType = Enum.Parse<DrawingFillType>(fillTypeElement.Value);
            }

            XElement? fillBitmapElement = ellipseDoc.Root.Element(ns + "FillBitmap");
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
            writer.WriteElementString("DrawRight", DrawRight.ToString());

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