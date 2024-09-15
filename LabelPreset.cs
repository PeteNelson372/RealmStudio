using System.Xml.Serialization;

namespace RealmStudio
{
    [XmlRoot("LabelPreset", Namespace = "RealmStudio", IsNullable = false)]
    public class LabelPreset
    {
        [XmlAttribute]
        public bool IsDefault { get; set; } = false;
        [XmlElement]
        public string PresetXmlFilePath { get; set; } = string.Empty;
        [XmlElement]
        public string LabelPresetName { get; set; } = string.Empty;
        [XmlElement]
        public string LabelPresetTheme { get; set; } = string.Empty;
        [XmlElement]
        public int LabelColor { get; set; } = Color.Empty.ToArgb();
        [XmlElement]
        public int LabelOutlineColor { get; set; } = Color.Empty.ToArgb();
        [XmlElement]
        public int LabelOutlineWidth { get; set; } = 0;
        [XmlElement]
        public int LabelGlowColor { get; set; } = Color.Empty.ToArgb();
        [XmlElement]
        public int LabelGlowStrength { get; set; } = 0;
        [XmlElement]
        public string LabelFontString = string.Empty;
    }
}
