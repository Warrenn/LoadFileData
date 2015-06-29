namespace LoadFileData.ContentReader.Settings
{
    public class TextReaderSettings
    {
        public TextReaderSettings()
        {
            RemoveWhiteSpace = true;
            CommentStrings = new[] { "\"", "'" };
        }
        public string[] CommentStrings { get; set; }
        public bool RemoveWhiteSpace { get; set; }
    }
}
