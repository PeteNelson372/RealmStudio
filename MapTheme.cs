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
using System.Xml.Serialization;

namespace RealmStudio
{
    [XmlRoot("maptheme", Namespace = "RealmStudio", IsNullable = false)]
    public class MapTheme
    {
        public string ThemeName { get; set; } = string.Empty;
        public string? ThemePath { get; set; }
        public bool IsDefaultTheme { get; set; } = false;
        public bool IsSystemTheme { get; set; } = false;
        public MapTexture? BackgroundTexture { get; set; }
        public MapTexture? OceanTexture { get; set; }
        public int? OceanTextureOpacity { get; set; }
        public XmlColor? OceanColor { get; set; } = Color.Empty;
        public List<XmlColor> OceanColorPalette { get; set; } = [];
        public XmlColor? LandformOutlineColor { get; set; } = Color.Empty;
        public int? LandformOutlineWidth { get; set; }
        public MapTexture? LandformTexture { get; set; }
        public string? LandShorelineStyle { get; set; }
        public XmlColor? LandformCoastlineColor { get; set; } = Color.Empty;
        public string? LandformCoastlineStyle { get; set; }
        public int? LandformCoastlineEffectDistance { get; set; }
        public List<XmlColor> LandformColorPalette { get; set; } = [];
        public XmlColor? FreshwaterColor { get; set; } = Color.Empty;
        public XmlColor? FreshwaterShorelineColor { get; set; } = Color.Empty;
        public int? RiverWidth { get; set; }
        public bool? RiverSourceFadeIn { get; set; }
        public List<XmlColor> FreshwaterColorPalette { get; set; } = [];
        public XmlColor? PathColor { get; set; } = Color.Empty;
        public int? PathWidth { get; set; }
        public string? PathStyle { get; set; }
        public XmlColor? VignetteColor { get; set; } = ColorTranslator.FromHtml("#C9977B");
        public int? VignetteStrength { get; set; }
        public XmlColor[] SymbolCustomColors { get; set; } = [Color.Empty, Color.Empty, Color.Empty, Color.Empty];
    }
}
