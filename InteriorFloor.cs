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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public class InteriorFloor : MapComponent, IXmlSerializable
    {
        public RealmStudioMap ParentMap { get; set; } = new RealmStudioMap();

        public string InteriorFloorName { get; set; } = string.Empty;

        public string InteriorFloorDescription { get; set; } = string.Empty;

        public Guid InteriorFloorGuid { get; set; } = Guid.NewGuid();

        public SKPath ContourPath { get; set; } = new SKPath();

        public SKPath DrawPath { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        // inner paths are used to paint an effect around the inside of the interior floor
        public SKPath InnerPath1 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        // outer paths are used to paint an effect around the outside of the landform
        public SKPath OuterPath1 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };


        public List<SKPoint> ContourPoints { get; set; } = [];

        public SKPaint InteriorFloorFillPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true,
            BlendMode = SKBlendMode.Src,
        };

        public SKPaint InteriorFloorGradientPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeWidth = 2,
            BlendMode = SKBlendMode.SrcATop
        };

        public SKPaint InteriorFloorOutlinePaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true,
            BlendMode = SKBlendMode.Src,
        };


        public SKShader? DashShader { get; set; }
        public SKShader? LineHatchBitmapShader { get; set; }
        public MapTexture? InteriorFloorTexture { get; set; }
        public MapTexture? InteriorWallTexture { get; set; }
        public Color InteriorFloorOutlineColor { get; set; } = Color.FromArgb(255, 62, 55, 40);
        public Color InteriorFloorBackgroundColor { get; set; } = Color.White;
        public Color InteriorFloorFillColor { get; set; } = ColorTranslator.FromHtml("#AC964F");
        public int InteriorFloorOutlineWidth { get; set; } = 2;
        public bool FillWithTexture { get; set; } = true;
        public bool IsModified { get; set; } = true;
        public bool IsDeleted { get; set; }

        public SKSurface? InteriorFloorRenderSurface { get; set; }
        public SKSurface? InteriorOutlineRenderSurface { get; set; }

        public InteriorFloor() { }

        public InteriorFloor(InteriorFloor original)
        {
            ParentMap = original.ParentMap;
            DrawPath = new(original.DrawPath);

            DashShader = original.DashShader;

            FillWithTexture = original.FillWithTexture;

            LineHatchBitmapShader = original.LineHatchBitmapShader;

        }

        #region RENDER
        public override void Render(SKCanvas canvas)
        {
            if (InteriorFloorRenderSurface == null)
            {
                return;
            }

            if (!IsModified)
            {
                canvas.DrawSurface(InteriorFloorRenderSurface, new SKPoint(0, 0));
            }
            else
            {
                SKCanvas floorCanvas = InteriorFloorRenderSurface.Canvas;

                floorCanvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

                floorCanvas.Clear(SKColors.Transparent);

                floorCanvas.DrawPath(DrawPath, InteriorFloorFillPaint);

                canvas.DrawSurface(InteriorFloorRenderSurface, new SKPoint(0, 0));

                IsModified = false;
            }
        }
        #endregion


        #region XML SERIALIZATION
        /******************************************************************************************************* 
        * XML SERIALIZATION METHODS
        *******************************************************************************************************/
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
            XDocument mapInteriorFloorDoc = XDocument.Parse(content);

            IEnumerable<XElement?> nameElemEnum = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorName"));
            if (nameElemEnum != null && nameElemEnum.Any() && nameElemEnum.First() != null)
            {
                string? name = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorName").Value).FirstOrDefault();
                InteriorFloorName = name;
            }
            else
            {
                InteriorFloorName = string.Empty;
            }

            IEnumerable<XElement?> guidElemEnum = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorGuid"));
            if (guidElemEnum != null && guidElemEnum.Any() && guidElemEnum.First() != null)
            {
                string? mapGuid = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorGuid").Value).FirstOrDefault();
                InteriorFloorGuid = Guid.Parse(mapGuid);
            }
            else
            {
                InteriorFloorGuid = Guid.NewGuid();
            }

            IEnumerable<XElement?> descrElemEnum = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorDescription"));
            if (descrElemEnum != null && descrElemEnum.Any() && descrElemEnum.First() != null)
            {
                string? description = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorDescription").Value).FirstOrDefault();
                InteriorFloorDescription = description;
            }
            else
            {
                InteriorFloorDescription = string.Empty;
            }

            IEnumerable<XElement?> fillWithTextureElem = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "FillWithTexture"));
            if (fillWithTextureElem != null && fillWithTextureElem.Any() && fillWithTextureElem.First() != null)
            {
                string? fillWithTexture = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "FillWithTexture").Value).FirstOrDefault();

                if (bool.TryParse(fillWithTexture, out bool boolFill))
                {
                    FillWithTexture = boolFill;
                }
                else
                {
                    FillWithTexture = true;
                }
            }
            else
            {
                FillWithTexture = true;
            }

            string txName = string.Empty;
            string txPath = string.Empty;

            IEnumerable<XElement?> floorTextureElem = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorTexture"));
            if (floorTextureElem != null && floorTextureElem.Any() && floorTextureElem.First() != null)
            {
                foreach (var item in floorTextureElem.Descendants())
                {
                    if (item.Name.LocalName == "TextureName")
                    {
                        txName = item.Value;
                    }
                    else if (item.Name.LocalName == "TexturePath")
                    {
                        txPath = item.Value;
                    }
                }
            }

            if (!string.IsNullOrEmpty(txName) && !string.IsNullOrEmpty(txPath))
            {
                InteriorFloorTexture = new(txName, txPath);

                Bitmap b = new(InteriorFloorTexture.TexturePath);
                Bitmap resizedBitmap = new(b, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                InteriorFloorTexture.TextureBitmap = new(resizedBitmap);

                // create and set a shader from the texture
                SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                    SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                InteriorFloorFillPaint.Shader = flpShader;
            }
            else
            {
                // texture is not set in the interior object, so use default
                //if (InteriorManager.InteriorMediator.InteriorFloorTextureList[InteriorManager.InteriorMediator.InteriorFloorTextureIndex].TextureBitmap == null)
                //{
                //    InteriorManager.InteriorMediator.InteriorFloorTextureList[InteriorManager.InteriorMediator.InteriorFloorTextureIndex].TextureBitmap =
                //        (Bitmap?)Bitmap.FromFile(InteriorManager.InteriorMediator.InteriorFloorTextureList[InteriorManager.InteriorMediator.InteriorFloorTextureIndex].TexturePath);
                //}

                //InteriorFloorTexture = InteriorManager.InteriorMediator.InteriorFloorTextureList[InteriorManager.InteriorMediator.InteriorFloorTextureIndex];

                if (InteriorFloorTexture.TextureBitmap != null)
                {
                    Bitmap resizedBitmap = new(InteriorFloorTexture.TextureBitmap, MapBuilder.MAP_DEFAULT_WIDTH, MapBuilder.MAP_DEFAULT_HEIGHT);

                    // create and set a shader from the texture
                    SKShader flpShader = SKShader.CreateBitmap(Extensions.ToSKBitmap(resizedBitmap),
                        SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);

                    InteriorFloorFillPaint.Shader = flpShader;
                }
            }

            if (!FillWithTexture || InteriorFloorTexture == null || InteriorFloorTexture.TextureBitmap == null)
            {
                InteriorFloorFillPaint.Shader?.Dispose();
                InteriorFloorFillPaint.Shader = null;

                SKShader flpShader = SKShader.CreateColor(InteriorFloorBackgroundColor.ToSKColor());
                InteriorFloorFillPaint.Shader = flpShader;
            }

            IEnumerable<XElement> colorElem = mapInteriorFloorDoc.Descendants(ns + "InteriorFloorOutlineColor");
            if (colorElem != null && colorElem.Any() && colorElem.First() != null)
            {
                string? color = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorOutlineColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    InteriorFloorOutlineColor = ColorTranslator.FromHtml(color);
                }
                else
                {
                    if (int.TryParse(color, out int colorArgb))
                    {
                        InteriorFloorOutlineColor = Color.FromArgb(colorArgb);
                    }
                    else
                    {
                        InteriorFloorOutlineColor = Color.FromArgb(255, 62, 55, 40);
                    }
                }
            }
            else
            {
                InteriorFloorOutlineColor = Color.FromArgb(255, 62, 55, 40);
            }

            InteriorFloorFillColor = Color.FromArgb(InteriorFloorOutlineColor.A / 4, InteriorFloorOutlineColor);

            IEnumerable<XElement?> widthElem = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorOutlineWidth"));
            if (widthElem != null && widthElem.Any() && widthElem.First() != null)
            {
                string? width = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorOutlineWidth").Value).FirstOrDefault();

                if (int.TryParse(width, out int n))
                {
                    InteriorFloorOutlineWidth = n;
                }
                else
                {
                    InteriorFloorOutlineWidth = 2;
                }
            }
            else
            {
                InteriorFloorOutlineWidth = 2;
            }

            IEnumerable<XElement> backgroundColorElem = mapInteriorFloorDoc.Descendants(ns + "InteriorFloorBackgroundColor");
            if (backgroundColorElem != null && backgroundColorElem.Any() && backgroundColorElem.First() != null)
            {
                string? color = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "InteriorFloorBackgroundColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    InteriorFloorBackgroundColor = ColorTranslator.FromHtml(color);
                }
                else if (int.TryParse(color, out int colorArgb))
                {
                    InteriorFloorBackgroundColor = Color.FromArgb(colorArgb);
                }
                else
                {
                    InteriorFloorBackgroundColor = Color.White;
                }
            }
            else
            {
                InteriorFloorBackgroundColor = Color.White;
            }

            InteriorFloorFillColor = Color.FromArgb(InteriorFloorBackgroundColor.A / 4, InteriorFloorBackgroundColor);


            IEnumerable<XElement?> pathElemEnum = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "ContourPath"));
            if (pathElemEnum != null && pathElemEnum.Any() && pathElemEnum.First() != null)
            {
                string? contourPath = mapInteriorFloorDoc.Descendants().Select(x => x.Element(ns + "ContourPath").Value).FirstOrDefault();
                ContourPath = SKPath.ParseSvgPathData(contourPath);
                DrawPath = new(ContourPath);
            }
            else
            {
                throw new Exception("InteriorFloor path is not set. Cannot load this map.");
            }

            IsModified = true;

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);

            // floor name
            writer.WriteStartElement("InteriorFloorName");
            writer.WriteString(InteriorFloorName);
            writer.WriteEndElement();

            writer.WriteStartElement("InteriorFloorDescription");
            writer.WriteString(InteriorFloorDescription);
            writer.WriteEndElement();

            // floor GUID
            writer.WriteStartElement("InteriorFloorGuid");
            writer.WriteString(InteriorFloorGuid.ToString());
            writer.WriteEndElement();

            if (InteriorFloorTexture != null)
            {
                // landform texture
                writer.WriteStartElement("InteriorFloorTexture");
                writer.WriteStartElement("TextureName");
                writer.WriteString(InteriorFloorTexture.TextureName);
                writer.WriteEndElement();
                writer.WriteStartElement("TexturePath");
                writer.WriteString(InteriorFloorTexture.TexturePath);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            // fill with texture
            writer.WriteStartElement("FillWithTexture");
            writer.WriteValue(FillWithTexture);
            writer.WriteEndElement();

            // outline color
            int outlinecolor = InteriorFloorOutlineColor.ToArgb();
            writer.WriteStartElement("InteriorFloorOutlineColor");
            writer.WriteValue(outlinecolor);
            writer.WriteEndElement();

            // background color
            int backgroundcolor = InteriorFloorBackgroundColor.ToArgb();
            writer.WriteStartElement("InteriorFloorBackgroundColor");
            writer.WriteValue(backgroundcolor);
            writer.WriteEndElement();

            // outline width
            writer.WriteStartElement("InteriorFloorOutlineWidth");
            writer.WriteValue(InteriorFloorOutlineWidth);
            writer.WriteEndElement();

            // interior floor contourPath path
            writer.WriteStartElement("ContourPath");
            string pathSvg = ContourPath.ToSvgPathData();
            writer.WriteValue(pathSvg);
            writer.WriteEndElement();
        }
        #endregion
    }
}
