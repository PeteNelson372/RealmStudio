namespace RealmStudio
{
    internal class NameGenerator : INameGenerator
    {
        public Guid NameGeneratorGuid { get; set; } = Guid.NewGuid();
        public string NameGeneratorName { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = true;
        public List<string> Column1 { get; set; } = [];
        public List<string> Column2 { get; set; } = [];
    }
}
