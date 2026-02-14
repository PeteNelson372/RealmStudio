using System.IO;

namespace RealmStudioX
{
    public sealed class XmlAssetMetadataLoader : IAssetMetadataLoader
    {
        public object? Load(string absolutePath)
        {
            // You can refine by detecting root element type
            return File.ReadAllText(absolutePath); // placeholder for now
        }
    }
}
