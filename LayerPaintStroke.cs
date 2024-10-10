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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    internal class LayerPaintStroke : MapComponent, IXmlSerializable
    {
        private readonly RealmStudioMap ParentMap;

        public Guid StrokeId { get; set; } = Guid.NewGuid();
        public List<LayerPaintStrokePoint> PaintStrokePoints { get; set; } = [];
        public SKColor StrokeColor { get; set; } = SKColor.Empty;
        public ColorPaintBrush PaintBrush { get; set; } = new ColorPaintBrush();
        public int BrushRadius { get; set; } = 0;

        private SKShader StrokeShader = SKShader.CreateEmpty();
        private readonly int MapLayerIdentifer = 0;
        private readonly bool Erase = false;

        private readonly SKPaint ShaderPaint;

        public LayerPaintStroke(RealmStudioMap parentMap, SKColor strokeColor, ColorPaintBrush colorPaintBrush, int brushRadius, int mapLayerIdentifier, bool erase = false)
        {
            ParentMap = parentMap;
            StrokeColor = strokeColor;
            PaintBrush = colorPaintBrush;
            BrushRadius = brushRadius;
            MapLayerIdentifer = mapLayerIdentifier;
            Erase = erase;

            if (MapLayerIdentifer == MapBuilder.OCEANTEXTUREOVERLAYLAYER)
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
            else if (MapLayerIdentifer == MapBuilder.LANDDRAWINGLAYER)
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
            else if (MapLayerIdentifer == MapBuilder.WATERDRAWINGLAYER)
            {
                ShaderPaint = PaintObjects.WaterColorPaint;
            }
            else
            {
                ShaderPaint = new()
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColor.Empty,
                };
            }
        }

        public void AddLayerPaintStrokePoint(SKPoint location)
        {
            LayerPaintStrokePoint sp = new(location, BrushRadius);
            PaintStrokePoints.Add(sp);
        }

        public override void Render(SKCanvas canvas)
        {
            // clip rendering to landforms or water features, depending on what map layer
            // the brush stroke is on; painting on the ocean layer is not clipped

            if (MapLayerIdentifer == MapBuilder.LANDDRAWINGLAYER)
            {
                // clip drawing to the outer path of landforms

                List<MapComponent> landformList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;
                SKPath clipPath = new();
                for (int i = 0; i < landformList.Count; i++)
                {
                    SKPath landformOutlinePath = ((Landform)landformList[i]).ContourPath;

                    if (landformOutlinePath != null && landformOutlinePath.PointCount > 0)
                    {
                        clipPath.AddPath(landformOutlinePath);
                    }
                }

                canvas.Save();
                canvas.ClipPath(clipPath);
            }
            else if (MapLayerIdentifer == MapBuilder.WATERDRAWINGLAYER)
            {
                // clip drawing to the outer path of water features and rivers

                List<MapComponent> waterFeatureList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.WATERLAYER).MapLayerComponents;
                SKPath clipPath = new();
                for (int i = 0; i < waterFeatureList.Count; i++)
                {
                    if (waterFeatureList[i] is WaterFeature feature)
                    {
                        SKPath waterFeatureOutlinePath = feature.ContourPath;

                        if (waterFeatureOutlinePath != null && waterFeatureOutlinePath.PointCount > 0)
                        {
                            clipPath.AddPath(waterFeatureOutlinePath);
                        }
                    }
                    else if (waterFeatureList[i] is River river)
                    {
                        SKPath? riverBoundaryPath = river.RiverBoundaryPath;

                        if (riverBoundaryPath != null && riverBoundaryPath.PointCount > 0)
                        {
                            clipPath.AddPath(riverBoundaryPath);
                        }
                    }
                }


                canvas.Save();
                canvas.ClipPath(clipPath);
            }

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
                }

                canvas.DrawCircle(point.StrokeLocation.X, point.StrokeLocation.Y, point.StrokeRadius, ShaderPaint);
            }

            if (MapLayerIdentifer == MapBuilder.LANDDRAWINGLAYER || MapLayerIdentifer == MapBuilder.WATERDRAWINGLAYER)
            {
                canvas.Restore();
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
