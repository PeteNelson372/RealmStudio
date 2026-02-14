using System.Xml.Linq;

namespace RealmStudioX
{
    public sealed class MapFrameXmlLoader : IAssetMetadataLoader
    {
        public object Load(string absolutePath)
        {
            var doc = XDocument.Load(absolutePath);
            var ns = XNamespace.Get("RealmStudio");
            var root = doc.Root!;

            return new NinePatchMetadata
            {
                StretchLeft = (float)root.Element(ns + "FrameCenterLeft")!,
                StretchTop = (float)root.Element(ns + "FrameCenterTop")!,
                StretchRight = (float)root.Element(ns + "FrameCenterRight")!,
                StretchBottom = (float)root.Element(ns + "FrameCenterBottom")!
            };
        }
    }
}
