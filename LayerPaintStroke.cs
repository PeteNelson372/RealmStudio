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
* contact@brookmonte.com
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
        public Guid StrokeId { get; set; } = Guid.NewGuid();
        public List<LayerPaintStrokePoint> PaintStrokePoints { get; set; } = [];
        public SKColor StrokeColor { get; set; } = SKColor.Empty;
        public ColorPaintBrush PaintBrush { get; set; } = new ColorPaintBrush();
        public int BrushRadius { get; set; } = 0;

        private SKShader StrokeShader = SKShader.CreateEmpty();
        private readonly int MapLayerIdentifer = 0;
        private readonly bool Erase = false;

        private readonly SKPaint ShaderPaint;

        public LayerPaintStroke(SKColor strokeColor, ColorPaintBrush colorPaintBrush, int brushRadius, int mapLayerIdentifier, bool erase = false)
        {
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
                ShaderPaint = PaintObjects.LandColorPaint;
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

                // clip rendering to landforms or water features, depending on what map layer
                // the brush stroke is on; painting on the ocean layer is not clipped
                if (MapLayerIdentifer == MapBuilder.OCEANTEXTUREOVERLAYLAYER)
                {
                    // no clipping on ocean painting
                    canvas.DrawCircle(point.StrokeLocation.X, point.StrokeLocation.Y, point.StrokeRadius, ShaderPaint);
                }
                else if (MapLayerIdentifer == MapBuilder.LANDDRAWINGLAYER)
                {
                    // TODO - clip to landform contour
                }
                else if (MapLayerIdentifer == MapBuilder.WATERDRAWINGLAYER)
                {
                    // TODO - clip to water feature (painted water feature or river) boundary
                }
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
