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
using RealmStudio.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Extensions = SkiaSharp.Views.Desktop.Extensions;

namespace RealmStudio
{
    public class LayerPaintStroke : MapComponent, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; }

        public Guid StrokeId { get; set; } = Guid.NewGuid();
        public List<LayerPaintStrokePoint> PaintStrokePoints { get; set; } = [];
        public SKColor StrokeColor { get; set; } = SKColor.Empty;
        public ColorPaintBrush PaintBrush { get; set; }
        public int BrushRadius { get; set; }
        public SKShader StrokeShader { get; set; } = SKShader.CreateEmpty();
        public int MapLayerIdentifier { get; set; }
        public bool Erase { get; set; }

        public bool Rendered { get; set; }

        public SKPaint ShaderPaint { get; set; }

        public SKSurface? RenderSurface { get; set; }

        public LayerPaintStroke()
        {
            ShaderPaint = new();
        }

        public LayerPaintStroke(RealmStudioMap parentMap, SKColor strokeColor, ColorPaintBrush colorPaintBrush, int brushRadius, int mapLayerIdentifier, bool erase = false)
        {
            ParentMap = parentMap;
            StrokeColor = strokeColor;
            PaintBrush = colorPaintBrush;
            BrushRadius = brushRadius;
            MapLayerIdentifier = mapLayerIdentifier;
            Erase = erase;
            Rendered = false;

            if (MapLayerIdentifier == MapBuilder.OCEANDRAWINGLAYER)
            {
                if (!Erase)
                {
                    ShaderPaint = PaintObjects.OceanPaint;
                }
                else
                {
                    ShaderPaint = PaintObjects.OceanEraserPaint;
                }
            }
            else if (MapLayerIdentifier == MapBuilder.LANDDRAWINGLAYER)
            {
                if (!Erase)
                {
                    ShaderPaint = PaintObjects.LandColorPaint;
                }
                else
                {
                    ShaderPaint = PaintObjects.LandColorEraserPaint;
                }
            }
            else if (MapLayerIdentifier == MapBuilder.WATERDRAWINGLAYER)
            {
                if (!Erase)
                {
                    ShaderPaint = PaintObjects.WaterColorPaint;
                }
                else
                {
                    ShaderPaint = PaintObjects.WaterColorEraserPaint;
                }

            }
            else
            {
                ShaderPaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    Color = strokeColor,
                    BlendMode = SKBlendMode.Src
                };
            }
        }

        public void AddLayerPaintStrokePoint(SKPoint location)
        {
            Rendered = false;

            LayerPaintStrokePoint sp = new(location, BrushRadius);
            PaintStrokePoints.Add(sp);
        }

        public override void Render(SKCanvas canvas)
        {
            if (ParentMap == null || RenderSurface == null) return;

            if (Rendered && !Erase)
            {
                canvas.DrawSurface(RenderSurface, new SKPoint(0, 0));
                return;
            }

            using (new SKAutoCanvasRestore(RenderSurface.Canvas))
            {
                RenderSurface?.Canvas.Clear(SKColors.Transparent);

                // clip rendering to landforms or water features, depending on what map layer
                // the brush stroke is on; painting on the ocean layer is not clipped

                if (MapLayerIdentifier == MapBuilder.LANDDRAWINGLAYER)
                {
                    if (Settings.Default.ClipLandformColoring)
                    {
                        // clip drawing to the outer path of landforms

                        List<MapComponent> landformList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;
                        SKPath clipPath = new();

                        // get the landform at the stroke location; when it is found, set the clip path to the landform outline path

                        bool foundLandform = false;
                        for (int i = 0; i < landformList.Count; i++)
                        {
                            if (landformList[i] is Landform lf)
                            {
                                SKPath landformOutlinePath = lf.ContourPath;

                                if (landformOutlinePath != null && landformOutlinePath.PointCount > 0)
                                {
                                    foreach (LayerPaintStrokePoint point in PaintStrokePoints)
                                    {
                                        if (landformOutlinePath.Contains(point.StrokeLocation.X, point.StrokeLocation.Y))
                                        {
                                            clipPath.AddPath(landformOutlinePath);
                                            foundLandform = true;
                                            break;
                                        }
                                    }
                                }

                                if (foundLandform)
                                {
                                    break;
                                }
                            }

                        }

                        if (clipPath.PointCount > 2)
                        {
                            RenderSurface?.Canvas.ClipPath(clipPath);
                        }
                    }
                }
                else if (MapLayerIdentifier == MapBuilder.WATERDRAWINGLAYER)
                {
                    // clip drawing to the outer path of water features and rivers

                    List<MapComponent> waterFeatureList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.WATERLAYER).MapLayerComponents;
                    SKPath clipPath = new();
                    for (int i = 0; i < waterFeatureList.Count; i++)
                    {
                        if (waterFeatureList[i] is WaterFeature feature)
                        {
                            SKPath waterFeatureOutlinePath = feature.OuterPath3;

                            if (waterFeatureOutlinePath != null && waterFeatureOutlinePath.PointCount > 0)
                            {
                                clipPath.AddPath(waterFeatureOutlinePath);
                            }
                        }
                        else if (waterFeatureList[i] is River river)
                        {
                            SKPath? riverBoundaryPath = river.ShorelinePath;

                            if (riverBoundaryPath != null && riverBoundaryPath.PointCount > 0)
                            {
                                clipPath.AddPath(riverBoundaryPath);
                            }
                        }
                    }

                    RenderSurface?.Canvas.ClipPath(clipPath);
                }

                // when erasing (painting with transparent color), painting to the RenderSurface canvas doesn't paint over
                // previously painted color; painting directly to the canvas that is passed in does (a Skia bug?)
                foreach (LayerPaintStrokePoint point in PaintStrokePoints)
                {
                    if (!Erase)
                    {
                        StrokeShader = SKShader.CreateColor(StrokeColor);

                        if (PaintBrush == ColorPaintBrush.SoftBrush)
                        {
                            StrokeShader.Dispose();
                            SKPoint gradientCenter = new(point.StrokeLocation.X, point.StrokeLocation.Y);
                            StrokeShader = SKShader.CreateRadialGradient(gradientCenter, BrushRadius, [StrokeColor, StrokeColor.WithAlpha(0)], SKShaderTileMode.Clamp);
                        }

                        ShaderPaint.Shader = StrokeShader;
                        RenderSurface?.Canvas.DrawCircle(point.StrokeLocation.X, point.StrokeLocation.Y, point.StrokeRadius, ShaderPaint);
                    }
                    else
                    {
                        canvas.DrawCircle(point.StrokeLocation.X, point.StrokeLocation.Y, point.StrokeRadius, ShaderPaint);
                    }
                }

                if (!Erase)
                {
                    canvas.DrawSurface(RenderSurface, new SKPoint(0, 0));
                    Rendered = true;
                }
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
            XDocument paintStrokeDoc = XDocument.Parse(content);

            XAttribute? idAttr = paintStrokeDoc.Root?.Attribute("StrokeId");
            if (idAttr != null)
            {
                StrokeId = Guid.Parse(idAttr.Value);
            }

            XAttribute? colorAttr = paintStrokeDoc.Root?.Attribute("StrokeColor");
            if (colorAttr != null)
            {
                int argbColor = int.Parse(colorAttr.Value);
                Color strokeColor = Color.FromArgb(argbColor);
                StrokeColor = strokeColor.ToSKColor();
            }

            XAttribute? radiusAttr = paintStrokeDoc.Root?.Attribute("BrushRadius");
            if (radiusAttr != null)
            {
                BrushRadius = int.Parse(radiusAttr.Value);
            }

            XAttribute? layerAttr = paintStrokeDoc.Root?.Attribute("MapLayerIdentifier");
            if (layerAttr != null)
            {
                MapLayerIdentifier = int.Parse(layerAttr.Value);
            }

            XAttribute? brushAttr = paintStrokeDoc.Root?.Attribute("PaintBrush");
            if (brushAttr != null)
            {
                PaintBrush = Enum.Parse<ColorPaintBrush>(brushAttr.Value);
            }

            XAttribute? eraseAttr = paintStrokeDoc.Root?.Attribute("Erase");
            if (eraseAttr != null)
            {
                Erase = bool.Parse(eraseAttr.Value);
            }

            IEnumerable<XElement?> strokePointsElem = paintStrokeDoc.Descendants().Select(x => x.Element(ns + "PaintStrokePoints"));
            if (strokePointsElem.Any() && strokePointsElem.First() != null)
            {
                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true
                };

                foreach (XElement? elem in strokePointsElem.Descendants())
                {
                    if (elem != null && elem.Name.LocalName == "LayerPaintStrokePoint")
                    {
                        string layerStrokePointPointString = elem.ToString();

                        using XmlReader pointReader = XmlReader.Create(new StringReader(layerStrokePointPointString), settings);
                        pointReader.Read();
                        LayerPaintStrokePoint lpsp = new();
                        lpsp.ReadXml(pointReader);

                        PaintStrokePoints.Add(lpsp);
                    }
                }
            }

        }

        public void WriteXml(XmlWriter writer)
        {
            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);

            // paint stroke GUID
            writer.WriteAttributeString("StrokeId", StrokeId.ToString());

            Color strokeColor = Extensions.ToDrawingColor(StrokeColor);
            writer.WriteAttributeString("StrokeColor", strokeColor.ToArgb().ToString());

            writer.WriteAttributeString("BrushRadius", BrushRadius.ToString());

            writer.WriteAttributeString("MapLayerIdentifier", MapLayerIdentifier.ToString());

            writer.WriteAttributeString("PaintBrush", PaintBrush.ToString());

            writer.WriteAttributeString("Erase", Erase.ToString());

            writer.WriteStartElement("PaintStrokePoints");
            foreach (LayerPaintStrokePoint point in PaintStrokePoints)
            {
                writer.WriteStartElement("LayerPaintStrokePoint");
                point.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
