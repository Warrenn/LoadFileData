namespace LoadFileData.ETLLayer.ContentReader.Settings
{
    public class DelimitedSettings : CsvSettings
    {
        public virtual string[] Delimiters { get; set; }
    }
}
