namespace RealmStudioX
{
    public sealed class AssetLoadProgress
    {
        public int ProcessedFiles { get; init; }
        public int EstimatedTotalFiles { get; init; }
        public int TotalFiles { get; init; }
        public string? CurrentFile { get; init; }

        public int Percentage =>
            EstimatedTotalFiles == 0
                ? 0
                : (int)((ProcessedFiles / (double)EstimatedTotalFiles) * 100);
    }

}
