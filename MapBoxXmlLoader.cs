using System.Xml.Linq;

namespace RealmStudioX
{
    public sealed class MapBoxXmlLoader : IAssetMetadataLoader
    {
        public object Load(string absolutePath)
        {
            var doc = XDocument.Load(absolutePath);
            var ns = XNamespace.Get("RealmStudio");
            var root = doc.Root!;

            return new NinePatchMetadata
            {
                StretchLeft = (float)root.Element(ns + "BoxCenterLeft")!,
                StretchTop = (float)root.Element(ns + "BoxCenterTop")!,
                StretchRight = (float)root.Element(ns + "BoxCenterRight")!,
                StretchBottom = (float)root.Element(ns + "BoxCenterBottom")!
            };
        }
    }

}
