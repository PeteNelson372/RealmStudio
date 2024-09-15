using System.Xml.Serialization;

namespace RealmStudio
{
    [XmlRoot("maptheme", Namespace = "RealmStudio", IsNullable = false)]
    public class MapTheme
    {
        public string? ThemeName { get; set; }
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
