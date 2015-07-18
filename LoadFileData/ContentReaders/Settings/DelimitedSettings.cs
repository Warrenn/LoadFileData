namespace LoadFileData.ContentReaders.Settings
{
    public class DelimitedSettings : TextReaderSettings
    {
        public DelimitedSettings()
        {
            Delimiters = new[] {",", @"\|"};
            CommentStrings = new[] { "\"", "'" };
        }

        public string[] CommentStrings { get; set; }

        public string[] Delimiters { get; set; }


    }
}
