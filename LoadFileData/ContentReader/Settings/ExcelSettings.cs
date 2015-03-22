namespace LoadFileData.ContentReader.Settings
{
    public class ExcelSettings : ContentReaderSettings
    {
        public virtual string SheetName { get; set; }
        public virtual int SheetNumber { get; set; }
    }
}
