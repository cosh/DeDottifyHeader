namespace DeDottifyHeader
{
    public class ProcessingSettings
    {
        public string TargetDirectory { get; set; } = string.Empty;
        public string FilePattern { get; set; } = "*.csv";
        public bool IncludeSubdirectories { get; set; } = true;
        public string ColumnSeparator { get; set; } = "|";
        public List<StringReplacement> Replacements { get; set; } = new();
    }
}