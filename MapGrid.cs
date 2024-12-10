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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapGrid : MapComponent, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; } = null;

        public Guid GridGuid { get; set; } = Guid.NewGuid();

        public bool GridEnabled { get; set; } = false;

        public GridTypeEnum GridType { get; set; } = GridTypeEnum.Square;

        public Color GridColor { get; set; } = Color.FromArgb(126, 0, 0, 0);

        public int GridLayerIndex { get; set; } = MapBuilder.DEFAULTGRIDLAYER;

        public int GridSize { get; set; } = 64;

        public int GridLineWidth { get; set; } = 2;

        public SKPaint? GridPaint { get; set; } = null;

        public bool ShowGridSize { get; set; } = true;

        public override void Render(SKCanvas canvas)
        {
            if (GridEnabled)
            {
                GridPaint ??= new()
                {
                    Style = SKPaintStyle.Stroke,
                    Color = GridColor.ToSKColor(),
                    StrokeWidth = GridLineWidth,
                    StrokeJoin = SKStrokeJoin.Bevel
                };

                switch (GridType)
                {
                    case GridTypeEnum.Square:
                        RenderSquareGrid(canvas);
                        break;

                    case GridTypeEnum.PointedHex:
                        RenderPointedHexGrid(canvas);
                        break;

                    case GridTypeEnum.FlatHex:
                        RenderFlatHexGrid(canvas);
                        break;
                }
            }
        }

        private void RenderSquareGrid(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            int numHorizontalLines = (int)(Height / GridSize);
            int numVerticalLines = (int)(Width / GridSize);

            int xOffset = GridSize;
            int yOffset = GridSize;

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

                for (int i = 0; i < numHorizontalLines; i++)
                {
                    SKPoint startPoint = new(0, yOffset);
                    SKPoint endPoint = new(Width, yOffset);
                    canvas.DrawLine(startPoint, endPoint, GridPaint);

                    yOffset += GridSize;
                }

                for (int j = 0; j < numVerticalLines; j++)
                {
                    SKPoint startPoint = new(xOffset, 0);
                    SKPoint endPoint = new(xOffset, Height);
                    canvas.DrawLine(startPoint, endPoint, GridPaint);

                    xOffset += GridSize;
                }

                if (ShowGridSize)
                {
                    float horizontalGridDistance = ParentMap.MapAreaWidth / numVerticalLines;
                    float verticalGridDistance = ParentMap.MapAreaHeight / numHorizontalLines;

                    string mapUnits = ParentMap.MapAreaUnits;

                    if (string.IsNullOrEmpty(mapUnits) || mapUnits.Length == 0)
                    {
                        mapUnits = "pixels";
                    }

                    string gridScaleString = string.Format("One square is {0:N} by {1:N} {2}", horizontalGridDistance, verticalGridDistance, mapUnits);


                    using SKPaint glowPaint = new()
                    {
                        Color = SKColors.Yellow,
                        MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Outer, 10),
                        IsAntialias = true,
                        TextSize = 10,
                        Typeface = SKTypeface.FromFamilyName(
                        familyName: "Tahoma",
                        weight: SKFontStyleWeight.Normal,
                        width: SKFontStyleWidth.Normal,
                        slant: SKFontStyleSlant.Upright)
                    };

                    canvas.DrawText(gridScaleString, 20, Height - 40, glowPaint);

                    using SKPaint paint = new()
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 10,
                        Typeface = SKTypeface.FromFamilyName(
                        familyName: "Tahoma",
                        weight: SKFontStyleWeight.Normal,
                        width: SKFontStyleWidth.Normal,
                        slant: SKFontStyleSlant.Upright)
                    };

                    canvas.DrawText(gridScaleString, 20, Height - 40, paint);
                }

            }
        }

        private void RenderPointedHexGrid(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            float hexwidth = GridSize * (float)Math.Sqrt(3.0);
            float hexheight = GridSize * 2.0F;
            float horizontalSpacing = hexwidth;
            float verticalSpacing = hexheight * 0.75F;

            // tile the grid onto the map
            SKPoint center = new(hexwidth, hexheight);

            int numHorizontalHexagons = (int)(Width / horizontalSpacing) + 2;
            int numVerticalHexagons = (int)(Height / verticalSpacing) + 2;

            using (new SKAutoCanvasRestore(canvas))
            {

                canvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

                for (int i = 0; i < numVerticalHexagons; i++)
                {
                    for (int j = 0; j < numHorizontalHexagons; j++)
                    {
                        SKPoint[] hexPoints = new SKPoint[6];

                        center.X = j * horizontalSpacing;
                        center.Y = i * verticalSpacing;

                        for (int k = 0; k < 6; k++)
                        {
                            float angle_deg = (60.0F * k) + 30.0F;
                            float angle_rad = (float)(Math.PI / 180.0F * angle_deg);

                            if (int.IsOddInteger(i))
                            {
                                hexPoints[k] = new SKPoint((float)(center.X + (horizontalSpacing / 2.0F) + GridSize * Math.Cos(angle_rad)),
                                    (float)(center.Y + GridSize * Math.Sin(angle_rad)));
                            }
                            else
                            {
                                hexPoints[k] = new SKPoint((float)(center.X + GridSize * Math.Cos(angle_rad)),
                                    (float)(center.Y + GridSize * Math.Sin(angle_rad)));
                            }

                        }

                        canvas.DrawPoints(SKPointMode.Lines, hexPoints, GridPaint);
                    }
                }

                if (ShowGridSize)
                {
                    float horizontalGridDistance = ParentMap.MapPixelWidth * hexwidth;
                    float verticalGridDistance = ParentMap.MapPixelHeight * hexheight;

                    string mapUnits = ParentMap.MapAreaUnits;

                    if (string.IsNullOrEmpty(mapUnits) || mapUnits.Length == 0)
                    {
                        mapUnits = "pixels";
                    }

                    string gridScaleString = string.Format("One hexagon is {0:N} by {1:N} {2}", horizontalGridDistance, verticalGridDistance, mapUnits);

                    using SKPaint glowPaint = new()
                    {
                        Color = SKColors.Yellow,
                        MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Outer, 10),
                        IsAntialias = true,
                        TextSize = 10,
                        Typeface = SKTypeface.FromFamilyName(
                        familyName: "Tahoma",
                        weight: SKFontStyleWeight.Normal,
                        width: SKFontStyleWidth.Normal,
                        slant: SKFontStyleSlant.Upright)
                    };

                    canvas.DrawText(gridScaleString, 20, Height - 40, glowPaint);

                    using SKPaint paint = new()
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 10,
                        Typeface = SKTypeface.FromFamilyName(
                        familyName: "Tahoma",
                        weight: SKFontStyleWeight.Normal,
                        width: SKFontStyleWidth.Normal,
                        slant: SKFontStyleSlant.Upright)
                    };

                    canvas.DrawText(gridScaleString, 20, Height - 40, paint);
                }
            }
        }

        private void RenderFlatHexGrid(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            float hexwidth = GridSize * 2.0F;
            float hexheight = GridSize * (float)Math.Sqrt(3.0);
            float horizontalSpacing = hexwidth * 0.75F;
            float verticalSpacing = hexheight;

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipRect(new SKRect(0, 0, ParentMap.MapWidth, ParentMap.MapHeight));

                // tile the grid onto the map
                SKPoint center = new(hexwidth, hexheight);

                int numHorizontalHexagons = (int)(Width / horizontalSpacing) + 2;
                int numVerticalHexagons = (int)(Height / verticalSpacing) + 2;

                for (int i = 0; i < numVerticalHexagons; i++)
                {
                    for (int j = 0; j < numHorizontalHexagons; j++)
                    {
                        SKPoint[] hexPoints = new SKPoint[6];

                        center.X = j * horizontalSpacing;
                        center.Y = i * verticalSpacing;

                        for (int k = 0; k < 6; k++)
                        {
                            float angle_deg = 60.0F * k;
                            float angle_rad = (float)(Math.PI / 180.0F * angle_deg);

                            if (int.IsOddInteger(j))
                            {
                                hexPoints[k] = new SKPoint((float)(center.X + GridSize * Math.Cos(angle_rad)),
                                    (float)(center.Y + (verticalSpacing / 2.0F) + GridSize * Math.Sin(angle_rad)));
                            }
                            else
                            {
                                hexPoints[k] = new SKPoint((float)(center.X + GridSize * Math.Cos(angle_rad)),
                                    (float)(center.Y + GridSize * Math.Sin(angle_rad)));
                            }

                        }

                        canvas.DrawPoints(SKPointMode.Lines, hexPoints, GridPaint);
                    }
                }

                if (ShowGridSize)
                {
                    float horizontalGridDistance = ParentMap.MapPixelWidth * hexwidth;
                    float verticalGridDistance = ParentMap.MapPixelHeight * hexheight;

                    string mapUnits = ParentMap.MapAreaUnits;

                    if (string.IsNullOrEmpty(mapUnits) || mapUnits.Length == 0)
                    {
                        mapUnits = "pixels";
                    }

                    string gridScaleString = string.Format("One hexagon is {0:N} by {1:N} {2}", horizontalGridDistance, verticalGridDistance, mapUnits);

                    using SKPaint glowPaint = new()
                    {
                        Color = SKColors.Yellow,
                        MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Outer, 10),
                        IsAntialias = true,
                        TextSize = 10,
                        Typeface = SKTypeface.FromFamilyName(
                        familyName: "Tahoma",
                        weight: SKFontStyleWeight.Normal,
                        width: SKFontStyleWidth.Normal,
                        slant: SKFontStyleSlant.Upright)
                    };

                    canvas.DrawText(gridScaleString, 20, Height - 40, glowPaint);

                    using SKPaint paint = new()
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 10,
                        Typeface = SKTypeface.FromFamilyName(
                        familyName: "Tahoma",
                        weight: SKFontStyleWeight.Normal,
                        width: SKFontStyleWidth.Normal,
                        slant: SKFontStyleSlant.Upright)
                    };

                    canvas.DrawText(gridScaleString, 20, Height - 40, paint);
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
            XDocument mapGridDoc = XDocument.Parse(content);

            XAttribute? xAttr = mapGridDoc.Root.Attribute("X");
            if (xAttr != null)
            {
                X = int.Parse(xAttr.Value);
            }

            XAttribute? yAttr = mapGridDoc.Root.Attribute("Y");
            if (yAttr != null)
            {
                Y = int.Parse(yAttr.Value);
            }

            XAttribute? wAttr = mapGridDoc.Root.Attribute("Width");
            if (wAttr != null)
            {
                Width = int.Parse(wAttr.Value);
            }

            XAttribute? hAttr = mapGridDoc.Root.Attribute("Height");
            if (hAttr != null)
            {
                Height = int.Parse(hAttr.Value);
            }

            IEnumerable<XElement?> guidElemEnum = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridGuid"));
            if (guidElemEnum.First() != null)
            {
                string? gridGuid = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridGuid").Value).FirstOrDefault();
                GridGuid = Guid.Parse(gridGuid);
            }

            IEnumerable<XElement?> typeElemEnum = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridType"));
            if (typeElemEnum.First() != null)
            {
                string? gridType = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridType").Value).FirstOrDefault();
                GridType = Enum.Parse<GridTypeEnum>(gridType);
            }

            IEnumerable<XElement> gridColorElem = mapGridDoc.Descendants(ns + "GridColor");
            if (gridColorElem != null && gridColorElem.Count() > 0 && gridColorElem.First() != null)
            {
                string? gridColor = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridColor").Value).FirstOrDefault();

                int argbValue = 0;

                if (gridColor.StartsWith("#"))
                {
                    argbValue = ColorTranslator.FromHtml(gridColor).ToArgb();
                }

                if (int.TryParse(gridColor, out int n))
                {
                    if (n > 0)
                    {
                        argbValue = n;
                    }
                    else
                    {
                        argbValue = Color.FromArgb(126, 0, 0, 0).ToArgb();
                    }
                }

                GridColor = Color.FromArgb(argbValue);
            }
            else
            {
                GridColor = Color.FromArgb(126, 0, 0, 0);
            }

            IEnumerable<XElement?> layerIndexElem = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridLayerIndex"));
            if (layerIndexElem.First() != null)
            {
                string? gridLayerIndex = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridLayerIndex").Value).FirstOrDefault();
                GridLayerIndex = int.Parse(gridLayerIndex);
            }

            IEnumerable<XElement?> gridSizeElem = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridSize"));
            if (gridSizeElem.First() != null)
            {
                string? gridSize = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridSize").Value).FirstOrDefault();
                GridSize = int.Parse(gridSize);
            }

            IEnumerable<XElement?> lineWidthElem = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridLineWidth"));
            if (lineWidthElem.First() != null)
            {
                string? gridLineWidth = mapGridDoc.Descendants().Select(x => x.Element(ns + "GridLineWidth").Value).FirstOrDefault();
                GridLineWidth = int.Parse(gridLineWidth);
            }

            GridEnabled = true;

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

            writer.WriteStartElement("GridGuid");
            writer.WriteString(GridGuid.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("GridType");
            writer.WriteString(GridType.ToString());
            writer.WriteEndElement();

            int gridColor = GridColor.ToArgb();
            writer.WriteStartElement("GridColor");
            writer.WriteValue(gridColor);
            writer.WriteEndElement();

            writer.WriteStartElement("GridLayerIndex");
            writer.WriteValue(GridLayerIndex);
            writer.WriteEndElement();

            writer.WriteStartElement("GridSize");
            writer.WriteValue(GridSize);
            writer.WriteEndElement();

            writer.WriteStartElement("GridLineWidth");
            writer.WriteValue(GridLineWidth);
            writer.WriteEndElement();
        }
    }
}
