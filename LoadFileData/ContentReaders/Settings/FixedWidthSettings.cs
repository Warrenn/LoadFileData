namespace LoadFileData.ContentReaders.Settings
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
