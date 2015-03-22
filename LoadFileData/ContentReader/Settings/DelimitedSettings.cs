namespace LoadFileData.ContentReader.Settings
{
    public class DelimitedSettings : TextReaderSettings
    {
        public DelimitedSettings()
        {
            Delimiters = new[] {",", @"\|"};
        }
        public string[] Delimiters { get; set; }
    }
}
