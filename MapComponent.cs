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
using System.Xml.Serialization;

namespace RealmStudio
{
    [XmlInclude(typeof(MapLayer))]
    public abstract class MapComponent : IMapComponent
    {
        [XmlIgnore]
        public bool RenderComponent { get; set; } = true;

        [XmlArray("MapLayerComponents")]
        [XmlArrayItem("Landform", Type = typeof(Landform))]
        [XmlArrayItem("WaterFeature", Type = typeof(WaterFeature))]
        [XmlArrayItem("MapPath", Type = typeof(MapPath))]
        [XmlArrayItem("MapSymbol", Type = typeof(MapSymbol))]
        [XmlArrayItem("River", Type = typeof(River))]
        [XmlArrayItem("MapLabel", Type = typeof(MapLabel))]
        [XmlArrayItem("PlacedMapBox", Type = typeof(PlacedMapBox))]
        [XmlArrayItem("PlacedMapFrame", Type = typeof(PlacedMapFrame))]
        [XmlArrayItem("MapGrid", Type = typeof(MapGrid))]
        [XmlArrayItem("Windrose", Type = typeof(MapWindrose))]
        [XmlArrayItem("MapScale", Type = typeof(MapScale))]
        [XmlArrayItem("MapRegion", Type = typeof(MapRegion))]
        [XmlArrayItem("MapVignette", Type = typeof(MapVignette))]
        public List<MapComponent> MapLayerComponents { get; } = new List<MapComponent>(500);

        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }

        [XmlAttribute]
        public int Width { get; set; }

        [XmlAttribute]
        public int Height { get; set; }

        public abstract void Render(SKCanvas canvas);
    }
}
