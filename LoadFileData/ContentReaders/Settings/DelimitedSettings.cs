namespace LoadFileData.ContentReaders.Settings
{
    public class DelimitedSettings : TextReaderSettings
    {
        public const string DefaultCommentsString = "\"'";
        public const string DefaultDelimitersString = "\\|,";

        public static string[] DefaultComments = { "\"", "'" };
        public static string[] DefaultDelimiters = { ",", "|" };

        public DelimitedSettings()
        {
            Delimiters = DefaultDelimiters;
            CommentStrings = DefaultComments;
        }

        public string[] CommentStrings { get; set; }

        public string[] Delimiters { get; set; }


    }
}
