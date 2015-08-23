namespace LoadFileData.ContentReaders.Settings
{
    public class ExcelSettings
    {
        public ExcelSettings()
            : this(null, null)
        {

        }

        public ExcelSettings(string range)
            : this(range, null)
        {

        }

        public ExcelSettings(string range, string sheet)
        {
            if (string.IsNullOrEmpty(range))
            {
                range = "A1";
            }
            if (string.IsNullOrEmpty(sheet))
            {
                sheet = "Sheet1";
            }
            Range = ExcelRangeFactory.CreateRange(range);
            Sheet = sheet;
        }

        public ExcelRange Range { get; set; }
        public string Sheet { get; set; }
    }
}
