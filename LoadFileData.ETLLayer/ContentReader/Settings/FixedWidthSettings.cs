namespace LoadFileData.ETLLayer.ContentReader.Settings
{
    public class FixedWidthSettings : CsvSettings
    {
        public virtual int[] FieldWidths { get; set; }
    }
}
