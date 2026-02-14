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
    public sealed class DrawnPolygon : DrawnMapComponent, IXmlSerializable
    {
        private List<SKPoint> _points = [];
        private SKColor _color = SKColors.Black;
        private SKColor _fillColor = SKColors.Transparent;
        private int _brushSize = 2;
        private int _rotation;
        private DrawingFillType _fillType = DrawingFillType.None;
        private Bitmap? _fillBitmap;
        private SKShader? _shader;

        public List<SKPoint> Points
        {
            get => _points;
            set
            {
                _points = value ?? throw new ArgumentNullException(nameof(value), "Points cannot be null.");
            }
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
                StrokeCap = SKStrokeCap.Round,
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

            if (Points.Count < 1)
            {
                return;
            }
            else if (Points.Count == 2)
            {
                canvas.DrawLine(Points[0], Points[1], paint);
            }
            else if (Points.Count > 2)
            {
                SKPath polyPath = DrawingMethods.GetLinePathFromPoints(Points);

                // set the bounds of the drawn polygon
                Bounds = polyPath.Bounds;
                Bounds = SKRect.Inflate(Bounds, 2, 2);

                using SKAutoCanvasRestore autoRestore = new(canvas, true);
                if (Rotation != 0)
                {
                    canvas.RotateDegrees(Rotation, Bounds.MidX, Bounds.MidY);
                }

                base.Render(canvas);

                // draw the filled polygon first if the fill is enabled
                if (FillType != DrawingFillType.None)
                {
                    // draw the filled rectangle first if the fill is enabled
                    canvas.DrawPath(polyPath, fillPaint);
                }

                // draw the outline of the polygon
                canvas.DrawPath(polyPath, paint);
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

            XElement? pointsElement = polyDoc.Root.Element(ns + "Points");
            if (pointsElement != null)
            {
                List<SKPoint> points = [];
                foreach (XElement pointElement in pointsElement.Elements(ns + "Point"))
                {
                    XElement? xElement = pointElement.Element(ns + "X");
                    XElement? yElement = pointElement.Element(ns + "Y");
                    if (xElement != null && yElement != null &&
                        float.TryParse(xElement.Value, out float x) &&
                        float.TryParse(yElement.Value, out float y))
                    {
                        points.Add(new SKPoint(x, y));
                    }
                }

                Points = points;

                int minX = (int)Points.Min(p => p.X);
                int minY = (int)Points.Min(p => p.Y);
                int maxX = (int)Points.Max(p => p.X);
                int maxY = (int)Points.Max(p => p.Y);

                X = minX;
                Y = minY;
                Width = maxX - minX;
                Height = maxY - minY;

                Bounds = new SKRect(minX, minY, maxX, maxY);
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

            writer.WriteElementString("Color", Color.ToString());
            writer.WriteElementString("BrushSize", BrushSize.ToString());
            writer.WriteElementString("Rotation", Rotation.ToString());
            writer.WriteElementString("FillType", FillType.ToString());

            writer.WriteStartElement("Points");
            for (int i = 0; i < Points.Count; i++)
            {
                writer.WriteStartElement("Point");
                writer.WriteAttributeString("Index", i.ToString());
                writer.WriteElementString("X", Points[i].X.ToString());
                writer.WriteElementString("Y", Points[i].Y.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // Points

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
