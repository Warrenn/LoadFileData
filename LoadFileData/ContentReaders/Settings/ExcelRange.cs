namespace LoadFileData.ContentReaders.Settings
{
    public class ExcelRange
    {
        public int ColumnStart { get; set; }
        public int RowStart { get; set; }
        public int? ColumnEnd { get; set; }
        public int? RowEnd { get; set; }

        public ExcelRange()
        {
            ColumnStart = 1;
            RowStart = 1;
            ColumnEnd = null;
            RowEnd = null;
        }
    }
}
