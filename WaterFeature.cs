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
using SkiaSharp.Views.Desktop;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RealmStudio
{
    internal class WaterFeature : MapComponent, IWaterFeature, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; } = null;
        public string WaterFeatureName { get; set; } = String.Empty;
        public Guid WaterFeatureGuid { get; set; } = Guid.NewGuid();
        public WaterFeatureTypeEnum WaterFeatureType { get; set; } = WaterFeatureTypeEnum.NotSet;
        public Color WaterFeatureColor { get; set; } = ColorTranslator.FromHtml("#658CBFC5");
        public Color WaterFeatureShorelineColor { get; set; } = ColorTranslator.FromHtml("#A19076");

        public int ShorelineEffectDistance { get; set; } = 6;

        public SKPath WaterFeaturePath { get; set; } = new()
        {
            FillType = SKPathFillType.Winding
        };

        public SKPath ContourPath { get; set; } = new()
        {
            FillType = SKPathFillType.Winding
        };

        public List<SKPoint> ContourPoints { get; set; } = [];

        // inner paths are used to paint the gradient shading around the inside of the water feature
        public SKPath InnerPath1 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath2 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath InnerPath3 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        // outer paths are used to paint the coastline effect around the outside of the water feature
        public SKPath OuterPath1 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath2 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public SKPath OuterPath3 { get; set; } = new SKPath()
        {
            FillType = SKPathFillType.Winding,
        };

        public bool IsSelected { get; set; } = false;

        public SKPaint WaterFeatureBackgroundPaint { get; set; } = new()
        {
            Color = ColorTranslator.FromHtml("#658CBFC5").ToSKColor(),
        };

        public SKPaint WaterFeatureShorelinePaint { get; set; } = new()
        {
            Color = Color.FromArgb(161, 144, 118).ToSKColor(),
        };

        public SKPaint ShallowWaterPaint { get; set; } = new()
        {
            Color = ColorTranslator.FromHtml("#658CBFC5").ToSKColor(),
        };

        public override void Render(SKCanvas canvas)
        {
            if (ParentMap == null) return;

            // clip the water feature drawing to the outer path of landforms
            List<MapComponent> landformList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            for (int i = 0; i < landformList.Count; i++)
            {
                SKPath landformOutlinePath = ((Landform)landformList[i]).ContourPath;

                if (landformOutlinePath != null && landformOutlinePath.PointCount > 0 && WaterFeaturePath.PointCount > 0)
                {
                    canvas.Save();
                    canvas.ClipPath(landformOutlinePath);

                    DrawWaterFeatureWithGradient(canvas);

                    canvas.Restore();
                }
            }

        }

        private void DrawWaterFeatureWithGradient(SKCanvas canvas)
        {
            //
            // draw waterFeature with gradient
            //

            // fill the water feature base path with color
            canvas.DrawPath(WaterFeaturePath, WaterFeatureBackgroundPaint);

            // draw the shallow water gradients
            byte alpha = 100;
            float luminance = WaterFeatureColor.GetBrightness();
            luminance = Math.Min(1.5F * luminance, 1.0F);

            // create the color from the background color, but with 150% luminosity and reduced alpha
            ShallowWaterPaint.Color = SKColor.FromHsl(WaterFeatureColor.GetHue(), WaterFeatureColor.GetSaturation() * 100F, luminance * 100F, alpha);

            // draw the 1st water gradient
            canvas.DrawPath(InnerPath1, ShallowWaterPaint);

            // draw the 2nd shallow water gradient
            alpha = 50;

            luminance = WaterFeatureColor.GetBrightness();
            luminance = Math.Min(1.25F * luminance, 1.0F);

            // create the color from the background color, but with 125% luminosity and reduced alpha
            ShallowWaterPaint.Color = SKColor.FromHsl(WaterFeatureColor.GetHue(), WaterFeatureColor.GetSaturation() * 100F, luminance * 100F, alpha);
            canvas.DrawPath(InnerPath2, ShallowWaterPaint);

            // draw the 3nd shallow water gradient
            alpha = 25;

            luminance = WaterFeatureColor.GetBrightness();

            // create the color from the background color, with 100% luminosity and reduced alpha
            ShallowWaterPaint.Color = SKColor.FromHsl(WaterFeatureColor.GetHue(), WaterFeatureColor.GetSaturation() * 100F, luminance * 100F, alpha);
            canvas.DrawPath(InnerPath3, ShallowWaterPaint);

            // draw the shoreline outer gradients
            alpha = 75;
            luminance = WaterFeatureColor.GetBrightness();
            luminance = Math.Min(0.9F * luminance, 1.0F);

            WaterFeatureShorelinePaint.Color = SKColor.FromHsl(WaterFeatureShorelineColor.GetHue(), WaterFeatureShorelineColor.GetSaturation() * 100F, luminance * 100F, alpha);
            canvas.DrawPath(OuterPath1, WaterFeatureShorelinePaint);

            WaterFeatureShorelinePaint.Color = WaterFeatureShorelineColor.ToSKColor();

            // draw the water feature border
            canvas.DrawPath(ContourPath, WaterFeatureShorelinePaint);
        }

        public XmlSchema? GetSchema()
        {
            throw new NotImplementedException();
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
