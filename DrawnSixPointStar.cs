﻿/**************************************************************************************************************************
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
    public sealed class DrawnSixPointStar : DrawnMapComponent, IXmlSerializable
    {
        private SKPoint _center;
        private float _radius;
        private SKColor _color = SKColors.Black;
        private SKColor _fillColor = SKColors.Transparent;
        private int _brushSize = 2;
        private int _rotation;
        private DrawingFillType _fillType = DrawingFillType.None;
        private Bitmap? _fillBitmap;
        private SKShader? _shader;

        public SKPoint Center
        {
            get => _center;
            set => _center = value;
        }

        public float Radius
        {
            get => _radius;
            set => _radius = value;
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

            List<SKPoint> points = [];

            for (int degrees = 0; degrees < 360; degrees += 30)
            {
                float radians = (float)(degrees * Math.PI / 180.0);

                if (degrees % 60 != 0)
                {
                    // every 60 degrees, we draw a point at the outer radius
                    float x = Center.X + Radius * (float)Math.Cos(radians);
                    float y = Center.Y + Radius * (float)Math.Sin(radians);
                    points.Add(new SKPoint(x, y));
                }
                else
                {
                    // every 30 degrees, we draw a point at the inner radius
                    float innerRadius = Radius / 2.0F;
                    float x = Center.X + innerRadius * (float)Math.Cos(radians);
                    float y = Center.Y + innerRadius * (float)Math.Sin(radians);
                    points.Add(new SKPoint(x, y));
                }
            }

            using SKPath path = new();

            path.MoveTo(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i]);
            }

            path.Close();
            Bounds = path.Bounds;

            using SKAutoCanvasRestore autoRestore = new(canvas, true);
            if (Rotation != 0)
            {
                canvas.RotateDegrees(Rotation, Bounds.MidX, Bounds.MidY);
            }

            base.Render(canvas);

            if (FillType != DrawingFillType.None)
            {
                // draw the filled star first if the fill is enabled
                canvas.DrawPath(path, fillPaint);
            }

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

            XDocument starDoc = XDocument.Parse(content);

            if (starDoc.Root == null)
            {
                throw new InvalidDataException("The six-point star XML data is missing the root element.");
            }

            XAttribute? xAttr = starDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = (int)float.Parse(xAttr.Value);
            }

            XAttribute? yAttr = starDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = (int)float.Parse(yAttr.Value);
            }

            XAttribute? wAttr = starDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = (int)float.Parse(wAttr.Value);
            }

            XAttribute? hAttr = starDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = (int)float.Parse(hAttr.Value);
            }

            XElement? centerElement = starDoc.Root.Element(ns + "Center");
            if (centerElement != null)
            {
                XAttribute? tlxAttr = centerElement.Attribute("X");
                XAttribute? tlyAttr = centerElement.Attribute("Y");
                if (tlxAttr != null && tlyAttr != null)
                {
                    Center = new SKPoint((int)float.Parse(tlxAttr.Value), (int)float.Parse(tlyAttr.Value));
                }
            }

            XElement? radiusElement = starDoc.Root.Element(ns + "Radius");
            if (radiusElement != null)
            {
                Radius = float.Parse(radiusElement.Value);
            }

            XElement? colorElement = starDoc.Root.Element(ns + "Color");
            if (colorElement != null)
            {
                _color = SKColor.Parse(colorElement.Value);
            }

            XElement? brushSizeElement = starDoc.Root.Element(ns + "BrushSize");
            if (brushSizeElement != null)
            {
                _brushSize = (int)float.Parse(brushSizeElement.Value);
            }

            XElement? rotationElement = starDoc.Root.Element(ns + "Rotation");
            if (rotationElement != null)
            {
                _rotation = (int)float.Parse(rotationElement.Value);
            }

            XElement? fillTypeElement = starDoc.Root.Element(ns + "FillType");
            if (fillTypeElement != null)
            {
                _fillType = Enum.Parse<DrawingFillType>(fillTypeElement.Value);
            }

            XElement? fillBitmapElement = starDoc.Root.Element(ns + "FillBitmap");
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

            writer.WriteStartElement("Center");
            writer.WriteAttributeString("X", Center.X.ToString());
            writer.WriteAttributeString("Y", Center.Y.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("Radius", Radius.ToString());
            writer.WriteElementString("Color", Color.ToString());
            writer.WriteElementString("BrushSize", BrushSize.ToString());
            writer.WriteElementString("Rotation", Rotation.ToString());
            writer.WriteElementString("FillType", FillType.ToString());

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
