namespace RealmStudio
{
    internal class NameBase : INameGenerator
    {
        public Guid NameBaseGuid { get; set; } = Guid.NewGuid();
        public string NameBaseName { get; set; } = string.Empty;
        public bool IsNameBaseSelected { get; set; } = true;
        public bool IsLanguageSelected { get; set; } = true;
        public string NameBaseLanguage { get; set; } = string.Empty;
        public int MinNameLength { get; set; } = 0;
        public int MaxNameLength { get; set; } = 0;
        public List<char> RepeatableCharacters { get; set; } = [];
        public float SingleWordTransformProportion { get; set; } = 0.0F;
        public List<string> NameStrings { get; set; } = [];
    }
}
