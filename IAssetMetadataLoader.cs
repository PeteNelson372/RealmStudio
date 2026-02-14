namespace RealmStudioX
{
    public interface IAssetMetadataLoader
    {
        /// <summary>
        /// Loads and parses an asset metadata file.
        /// The returned object must be a typed metadata instance
        /// understood by the engine (e.g. NinePatchMetadata).
        /// </summary>
        /// <param name="absolutePath">
        /// Absolute path to the XML metadata file.
        /// </param>
        /// <returns>
        /// A typed metadata object, or null if the file cannot be parsed.
        /// </returns>
        object? Load(string absolutePath);
    }

}
