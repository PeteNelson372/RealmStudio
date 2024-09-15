using System.Xml.Serialization;

namespace RealmStudio
{
    public class MapTexture
    {
        public MapTexture()
        {
            TextureName = string.Empty;
            TexturePath = string.Empty;
        }

        public MapTexture(string name, string path)
        {
            TextureName = name;
            TexturePath = path;
        }

        public string TextureName { get; set; }

        public string TexturePath { get; set; }

        [XmlIgnore]
        public Bitmap? TextureBitmap { get; set; }
    }
}
