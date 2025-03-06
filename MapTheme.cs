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
        public bool IsDefaultTheme { get; set; }
        public bool IsSystemTheme { get; set; }
        [XmlElement(IsNullable = true)] public MapTexture? BackgroundTexture { get; set; }
        [XmlElement(IsNullable = true)] public MapTexture? OceanTexture { get; set; }
        [XmlElement(IsNullable = true)] public int? OceanTextureOpacity { get; set; } = 255;
        [XmlElement(IsNullable = true)] public int? OceanColor { get; set; } = Color.LightGray.ToArgb();
        [XmlArray(IsNullable = true)] public List<int?>? OceanColorPalette { get; set; } = [];
        [XmlElement(IsNullable = true)] public int? LandformOutlineColor { get; set; } = Color.FromArgb(255, 62, 55, 40).ToArgb();
        [XmlElement(IsNullable = true)] public int? LandformBackgroundColor { get; set; } = Color.White.ToArgb();
        [XmlElement(IsNullable = true)] public int? LandformOutlineWidth { get; set; } = 2;
        [XmlElement(IsNullable = true)] public MapTexture? LandformTexture { get; set; }
        [XmlElement(IsNullable = true)] public bool? FillLandformWithTexture { get; set; } = true;
        [XmlElement(IsNullable = true)] public string? LandShorelineStyle { get; set; } = string.Empty;
        [XmlElement(IsNullable = true)] public int? LandformCoastlineColor { get; set; } = ColorTranslator.FromHtml("#BB9CC3B7").ToArgb();
        [XmlElement(IsNullable = true)] public string? LandformCoastlineStyle { get; set; } = string.Empty;
        [XmlElement(IsNullable = true)] public int? LandformCoastlineEffectDistance { get; set; } = 12;
        [XmlArray(IsNullable = true)] public List<int?>? LandformColorPalette { get; set; } = [];
        [XmlElement(IsNullable = true)] public int? FreshwaterColor { get; set; } = Color.FromArgb(101, 140, 191, 197).ToArgb();
        [XmlElement(IsNullable = true)] public int? FreshwaterShorelineColor { get; set; } = Color.Empty.ToArgb();
        [XmlElement(IsNullable = true)] public int? RiverWidth { get; set; } = 4;
        [XmlElement(IsNullable = true)] public bool? RiverSourceFadeIn { get; set; } = true;
        [XmlArray(IsNullable = true)] public List<int?>? FreshwaterColorPalette { get; set; } = [];
        [XmlElement(IsNullable = true)] public int? PathColor { get; set; } = Color.Empty.ToArgb();
        [XmlElement(IsNullable = true)] public int? PathWidth { get; set; } = 8;
        [XmlElement(IsNullable = true)] public PathTypeEnum? PathStyle { get; set; } = PathTypeEnum.SolidLinePath;
        [XmlElement(IsNullable = true)] public string? LabelFont { get; set; } = string.Empty;
        [XmlElement(IsNullable = true)] public int? LabelColor { get; set; } = ColorTranslator.FromHtml("#3D351E").ToArgb();
        [XmlElement(IsNullable = true)] public int? LabelOutlineColor { get; set; } = ColorTranslator.FromHtml("#A1D6CAAB").ToArgb();
        [XmlElement(IsNullable = true)] public float? LabelOutlineWidth { get; set; }
        [XmlElement(IsNullable = true)] public int? LabelGlowColor { get; set; } = Color.White.ToArgb();
        [XmlElement(IsNullable = true)] public int? LabelGlowStrength { get; set; }
        [XmlElement(IsNullable = true)] public int? VignetteColor { get; set; } = ColorTranslator.FromHtml("#C9977B").ToArgb();
        [XmlElement(IsNullable = true)] public int? VignetteStrength { get; set; } = 148;
        [XmlElement(IsNullable = true)] public bool? RectangleVignette { get; set; } = false;
        [XmlArray(IsNullable = true)] public int?[]? SymbolCustomColors { get; set; } = [Color.Empty.ToArgb(), Color.Empty.ToArgb(), Color.Empty.ToArgb(), Color.Empty.ToArgb()];

    }
}
