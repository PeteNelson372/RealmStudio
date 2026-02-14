namespace RealmStudioX
{
    public sealed class NinePatchMetadata
    {
        // Distances from bitmap edges to the stretchable center region
        public float StretchLeft { get; init; }
        public float StretchTop { get; init; }
        public float StretchRight { get; init; }
        public float StretchBottom { get; init; }
    }
}
