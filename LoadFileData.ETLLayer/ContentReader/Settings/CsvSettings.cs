namespace LoadFileData.ETLLayer.ContentReader.Settings
{
    public class CsvSettings : ContentReaderSettings
    {
        public virtual string[] CommentTokens { set; get; }
        public virtual bool TrimWhiteSpace { set; get; }
        public virtual bool HasFieldsEnclosedInQuotes { get; set; }
    }
}
