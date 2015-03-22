namespace LoadFileData.ContentReader.Settings
{
    public class FixedWidthSettings : TextReaderSettings
    {
        public FixedWidthSettings()
        {
            FieldWidths = new int[] {};
        }

        public int[] FieldWidths { get; set; }
    }
}
