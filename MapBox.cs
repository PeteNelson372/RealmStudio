using System.Xml.Serialization;

namespace RealmStudio
{
    [XmlRoot("mapbox", Namespace = "RealmStudio", IsNullable = false)]
    public class MapBox
    {
        public string? BoxName { get; set; }

        public Bitmap? BoxBitmap { get; set; }

        public string? BoxBitmapPath { get; set; }

        public string? BoxXmlFilePath { get; set; }

        public float BoxCenterLeft { get; set; } = 0;
        public float BoxCenterTop { get; set; } = 0;
        public float BoxCenterRight { get; set; } = 0;
        public float BoxCenterBottom { get; set; } = 0;
    }
}
