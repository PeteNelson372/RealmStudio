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
    public class WaterFeature : MapComponent, IWaterFeature, IXmlSerializable
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

            using SKPath clipPath = new();

            for (int i = 0; i < landformList.Count; i++)
            {
                SKPath landformOutlinePath = ((Landform)landformList[i]).ContourPath;

                if (landformOutlinePath != null && landformOutlinePath.PointCount > 0 && WaterFeaturePath.PointCount > 0)
                {
                    clipPath.AddPath(landformOutlinePath);
                }
            }

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipPath(clipPath);
                DrawWaterFeatureWithGradient(canvas);
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

            alpha = 192;
            Color shorelineColor = Color.FromArgb(alpha, WaterFeatureShorelinePaint.Color.ToDrawingColor());
            WaterFeatureShorelinePaint.Color = shorelineColor.ToSKColor();

            // draw the water feature border
            canvas.DrawPath(ContourPath, WaterFeatureShorelinePaint);

            // draw the shoreline outer gradients
            alpha = 128;
            shorelineColor = Color.FromArgb(alpha, WaterFeatureShorelinePaint.Color.ToDrawingColor());
            WaterFeatureShorelinePaint.Color = shorelineColor.ToSKColor();
            canvas.DrawPath(OuterPath1, WaterFeatureShorelinePaint);

            alpha = 64;
            shorelineColor = Color.FromArgb(alpha, WaterFeatureShorelinePaint.Color.ToDrawingColor());
            WaterFeatureShorelinePaint.Color = shorelineColor.ToSKColor();
            canvas.DrawPath(OuterPath2, WaterFeatureShorelinePaint);

            alpha = 32;
            shorelineColor = Color.FromArgb(alpha, WaterFeatureShorelinePaint.Color.ToDrawingColor());
            WaterFeatureShorelinePaint.Color = shorelineColor.ToSKColor();
            canvas.DrawPath(OuterPath3, WaterFeatureShorelinePaint);
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
            XDocument mapWaterFeatureDoc = XDocument.Parse(content);

            IEnumerable<XNode> nodes = mapWaterFeatureDoc.Descendants();

            IEnumerable<XElement?> nameElemEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureName"));
            if (nameElemEnum.First() != null)
            {
                string? mapWaterFeatureName = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureName").Value).FirstOrDefault();
                WaterFeatureName = mapWaterFeatureName;
            }

            IEnumerable<XElement?> guidElemEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureGuid"));
            if (guidElemEnum.First() != null)
            {
                string? mapGuid = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureGuid").Value).FirstOrDefault();
                WaterFeatureGuid = Guid.Parse(mapGuid);
            }

            IEnumerable<XElement?> typeElemEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureType"));
            if (typeElemEnum.First() != null)
            {
                string? waterFeatureType = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureType").Value).FirstOrDefault();
                WaterFeatureType = Enum.Parse<WaterFeatureTypeEnum>(waterFeatureType);
            }

            IEnumerable<XElement?> shorelineEffectDistanceEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "ShorelineEffectDistance"));
            if (shorelineEffectDistanceEnum.First() != null)
            {
                string? shorelineEffectDistance = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "ShorelineEffectDistance").Value).FirstOrDefault();
                ShorelineEffectDistance = int.Parse(shorelineEffectDistance);
            }

            IEnumerable<XElement?> colorElemEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureColor"));
            if (colorElemEnum.First() != null)
            {
                string? waterFeatureColor = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureColor").Value).FirstOrDefault();
                WaterFeatureColor = ColorTranslator.FromHtml(waterFeatureColor);
            }

            IEnumerable<XElement?> shoreColorElemEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureShorelineColor"));
            if (shoreColorElemEnum.First() != null)
            {
                string? waterFeatureShorelineColor = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeatureShorelineColor").Value).FirstOrDefault();
                WaterFeatureShorelineColor = ColorTranslator.FromHtml(waterFeatureShorelineColor);
            }

            IEnumerable<XElement?> pathElemEnum = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeaturePath"));
            if (pathElemEnum.First() != null)
            {
                string? waterFeaturePath = mapWaterFeatureDoc.Descendants().Select(x => x.Element(ns + "WaterFeaturePath").Value).FirstOrDefault();
                WaterFeaturePath = SKPath.ParseSvgPathData(waterFeaturePath);
            }

#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void WriteXml(XmlWriter writer)
        {
            using MemoryStream ms = new();
            using SKManagedWStream wstream = new(ms);

            // water feature name
            writer.WriteStartElement("WaterFeatureName");
            writer.WriteString(WaterFeatureName);
            writer.WriteEndElement();

            // water feature GUID
            writer.WriteStartElement("WaterFeatureGuid");
            writer.WriteString(WaterFeatureGuid.ToString());
            writer.WriteEndElement();

            // water feature type
            writer.WriteStartElement("WaterFeatureType");
            writer.WriteString(WaterFeatureType.ToString());
            writer.WriteEndElement();

            // shoreline effect distance
            writer.WriteStartElement("ShorelineEffectDistance");
            writer.WriteString(ShorelineEffectDistance.ToString());
            writer.WriteEndElement();

            // water feature color
            XmlColor waterfeaturecolor = new(WaterFeatureColor);
            writer.WriteStartElement("WaterFeatureColor");
            waterfeaturecolor.WriteXml(writer);
            writer.WriteEndElement();

            // water feature shoreline color
            XmlColor waterFeatureShorelineColor = new(WaterFeatureShorelineColor);
            writer.WriteStartElement("WaterFeatureShorelineColor");
            waterfeaturecolor.WriteXml(writer);
            writer.WriteEndElement();

            // water feature path
            writer.WriteStartElement("WaterFeaturePath");
            string pathSvg = WaterFeaturePath.ToSvgPathData();
            writer.WriteValue(pathSvg);
            writer.WriteEndElement();
        }


    }
}
