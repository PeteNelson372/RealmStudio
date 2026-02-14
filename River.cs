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
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public class River : MapComponent, IWaterFeature, IXmlSerializable
    {
        public RealmStudioMap? ParentMap { get; set; }

        public Guid MapRiverGuid { get; set; } = Guid.NewGuid();

        public string MapRiverName { get; set; } = "";

        public string MapRiverDescription { get; set; } = string.Empty;

        public List<MapRiverPoint> RiverPoints { get; set; } = [];

        public Color RiverColor { get; set; } = ColorTranslator.FromHtml("#839690");

        public float RiverWidth { get; set; } = 4;

        public int ShorelineEffectDistance { get; set; } = 6;

        public bool RiverSourceFadeIn { get; set; }

        public bool RenderRiverTexture { get; set; } = true;

        public Color RiverShorelineColor { get; set; } = Color.FromArgb(161, 144, 118);

        public bool ShowRiverPoints { get; set; }

        public SKPaint RiverFillPaint { get; set; } = new();

        public SKPaint RiverShorelinePaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.FromArgb(161, 144, 118).ToSKColor(),
        };

        public SKPaint RiverShallowWaterPaint { get; set; } = new()
        {
            Style = SKPaintStyle.Stroke,
        };

        public SKPath? RiverPath { get; set; }
        public SKPath? RiverBoundaryPath { get; set; }
        public SKPath? ShorelinePath { get; set; }
        public SKPath? Gradient1Path { get; set; }
        public SKPath? ShallowWaterPath { get; set; }

        public River() { }

        public River(River original)
        {
            X = original.X;
            Y = original.Y;
            Width = original.Width;
            Height = original.Height;
            MapRiverName = original.MapRiverName;
            ParentMap = original.ParentMap;
            RenderRiverTexture = original.RenderRiverTexture;
            RiverColor = original.RiverColor;
            RiverFillPaint = original.RiverFillPaint.Clone();
            RiverShallowWaterPaint = original.RiverShallowWaterPaint.Clone();
            RiverShorelineColor = original.RiverShorelineColor;
            RiverShorelinePaint = original.RiverShorelinePaint.Clone();
            RiverSourceFadeIn = original.RiverSourceFadeIn;
            RiverWidth = original.RiverWidth;
            ShorelineEffectDistance = original.ShorelineEffectDistance;
        }

        public override void Render(SKCanvas canvas)
        {
            /*
            if (ParentMap == null || RiverBoundaryPath == null
                || ShorelinePath == null || Gradient1Path == null || ShallowWaterPath == null)
            {
                return;
            }

            // clip the river drawing to the outer path of landforms
            List<MapComponent> landformList = MapBuilder.GetMapLayerByIndex(ParentMap, MapBuilder.LANDFORMLAYER).MapLayerComponents;

            using SKPath clipPath = new();

            for (int i = 0; i < landformList.Count; i++)
            {
                if (landformList[i] is Landform landform)
                {
                    SKPath landformOutlinePath = landform.ContourPath;

                    if (landformOutlinePath != null && landformOutlinePath.PointCount > 0)
                    {
                        clipPath.AddPath(landformOutlinePath);
                    }
                }
            }

            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.ClipPath(clipPath);

                // use multiple paths and multiple sets of parallel points and paint objects
                // to draw lines as gradients to shade rivers

                // shoreline
                byte alpha = 192;
                Color shorelineColor = Color.FromArgb(alpha, RiverShorelinePaint.Color.ToDrawingColor());
                RiverShorelinePaint.Color = shorelineColor.ToSKColor();

                canvas.DrawPath(ShorelinePath, RiverShorelinePaint);

                // shoreline gradient 1
                alpha = 128;
                shorelineColor = Color.FromArgb(alpha, RiverShorelinePaint.Color.ToDrawingColor());
                RiverShorelinePaint.Color = shorelineColor.ToSKColor();

                canvas.DrawPath(Gradient1Path, RiverShorelinePaint);

                // shoreline gradient 2
                //alpha = 64;
                //shorelineColor = Color.FromArgb(alpha, RiverShorelinePaint.Color.ToDrawingColor());
                //RiverShorelinePaint.Color = shorelineColor.ToSKColor();

                //canvas.DrawPath(Gradient2Path, RiverShorelinePaint);

                // shoreline gradient 3
                //alpha = 32;
                //shorelineColor = Color.FromArgb(alpha, RiverShorelinePaint.Color.ToDrawingColor());
                //RiverShorelinePaint.Color = shorelineColor.ToSKColor();
                //canvas.DrawPath(Gradient3Path, RiverShorelinePaint);

                canvas.DrawPath(RiverBoundaryPath, RiverFillPaint);

                canvas.DrawPath(ShallowWaterPath, RiverShallowWaterPaint);
            }

            */
        }

        public List<MapRiverPoint> GetRiverControlPoints()
        {
            List<MapRiverPoint> mapRiverPoints = [];

            // every 15th point is a control point
            for (int i = 0; i < RiverPoints.Count - 15; i += 15)
            {
                mapRiverPoints.Add(RiverPoints[i]);
                RiverPoints[i].IsControlPoint = true;
            }

            mapRiverPoints.Add(RiverPoints[^1]);
            RiverPoints[^1].IsControlPoint = true;

            return mapRiverPoints;
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
            XDocument mapRiverDoc = XDocument.Parse(content);

            IEnumerable<XElement?> nameElemEnum = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverName"));
            if (nameElemEnum.First() != null)
            {
                string? mapRiverName = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverName").Value).FirstOrDefault();
                MapRiverName = mapRiverName;
            }

            IEnumerable<XElement?> guidElemEnum = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverGuid"));
            if (guidElemEnum.First() != null)
            {
                string? mapGuid = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverGuid").Value).FirstOrDefault();
                MapRiverGuid = Guid.Parse(mapGuid);
            }

            IEnumerable<XElement?> descrElemEnum = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverDescription"));
            if (descrElemEnum != null && descrElemEnum.Any() && descrElemEnum.First() != null)
            {
                string? description = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverDescription").Value).FirstOrDefault();
                MapRiverDescription = description;
            }
            else
            {
                MapRiverDescription = string.Empty;
            }

            IEnumerable<XElement?> riverColorElem = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverColor"));
            if (riverColorElem != null && riverColorElem.Any() && riverColorElem.First() != null)
            {
                string? color = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    RiverColor = ColorTranslator.FromHtml(color);
                }
                else
                {
                    if (int.TryParse(color, out int colorArgb))
                    {
                        RiverColor = Color.FromArgb(colorArgb);
                    }
                    else
                    {
                        RiverColor = ColorTranslator.FromHtml("#839690");
                    }
                }
            }
            else
            {
                RiverColor = ColorTranslator.FromHtml("#839690");
            }

            IEnumerable<XElement?> riverWidthElem = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverWidth"));
            if (riverWidthElem.First() != null)
            {
                string? riverWidth = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverWidth").Value).FirstOrDefault();
                RiverWidth = int.Parse(riverWidth);
            }


            IEnumerable<XElement?> riverSourceFadeInElem = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverSourceFadeIn"));
            if (riverSourceFadeInElem.First() != null)
            {
                string? riverSourceFadeIn = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverSourceFadeIn").Value).FirstOrDefault();
                RiverSourceFadeIn = bool.Parse(riverSourceFadeIn);
            }
            else
            {
                RiverSourceFadeIn = true;
            }

            IEnumerable<XElement?> renderRiverTextureElem = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RenderRiverTexture"));
            if (renderRiverTextureElem.First() != null)
            {
                string? renderRiverTexture = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RenderRiverTexture").Value).FirstOrDefault();
                RenderRiverTexture = bool.Parse(renderRiverTexture);
            }
            else
            {
                RenderRiverTexture = true;
            }

            IEnumerable<XElement?> riverShorelineColorElem = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverShorelineColor"));
            if (riverShorelineColorElem != null && riverShorelineColorElem.Any() && riverShorelineColorElem.First() != null)
            {
                string? color = mapRiverDoc.Descendants().Select(x => x.Element(ns + "RiverShorelineColor").Value).FirstOrDefault();

                if (color.StartsWith('#'))
                {
                    RiverShorelineColor = ColorTranslator.FromHtml(color);
                }
                else
                {
                    if (int.TryParse(color, out int colorArgb))
                    {
                        RiverShorelineColor = Color.FromArgb(colorArgb);
                    }
                    else
                    {
                        RiverShorelineColor = Color.FromArgb(161, 144, 118);
                    }
                }
            }
            else
            {
                RiverShorelineColor = Color.FromArgb(161, 144, 118);
            }

            IEnumerable<XElement?> mapPointsElem = mapRiverDoc.Descendants().Select(x => x.Element(ns + "MapRiverPoints"));
            if (mapPointsElem.Any() && mapPointsElem.First() != null)
            {
                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true
                };

                foreach (XElement? elem in mapPointsElem.Descendants())
                {
                    if (elem != null && elem.Name.LocalName == "MapRiverPoint")
                    {
                        string mapPointString = elem.ToString();

                        using XmlReader pointReader = XmlReader.Create(new StringReader(mapPointString), settings);
                        pointReader.Read();
                        MapRiverPoint mpp = new();
                        mpp.ReadXml(pointReader);

                        RiverPoints.Add(mpp);
                    }
                }
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
            writer.WriteStartElement("MapRiverName");
            writer.WriteString(MapRiverName);
            writer.WriteEndElement();

            // water feature GUID
            writer.WriteStartElement("MapRiverGuid");
            writer.WriteString(MapRiverGuid.ToString());
            writer.WriteEndElement();

            // water feature description
            writer.WriteStartElement("MapRiverDescription");
            writer.WriteString(MapRiverDescription);
            writer.WriteEndElement();

            // water feature color
            writer.WriteStartElement("RiverColor");
            writer.WriteValue(RiverColor.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("RiverWidth");
            writer.WriteValue(RiverWidth.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RiverSourceFadeIn");
            writer.WriteValue(RiverSourceFadeIn.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RenderRiverTexture");
            writer.WriteValue(RenderRiverTexture.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RiverShorelineColor");
            writer.WriteValue(RiverShorelineColor.ToArgb());
            writer.WriteEndElement();

            writer.WriteStartElement("MapRiverPoints");
            foreach (MapRiverPoint point in RiverPoints)
            {
                writer.WriteStartElement("MapRiverPoint");
                point.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
