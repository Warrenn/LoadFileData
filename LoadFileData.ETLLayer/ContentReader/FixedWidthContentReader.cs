using LoadFileData.ETLLayer.ContentReader.Settings;
using Microsoft.VisualBasic.FileIO;

namespace LoadFileData.ETLLayer.ContentReader
{
    public class FixedWidthContentReader : CsvContentReaderBase
    {

        public override void ApplySettings(TextFieldParser parser, CsvSettings settings)
        {
            var fixedSettings = (FixedWidthSettings) settings;
            parser.TextFieldType = FieldType.FixedWidth;
            parser.FieldWidths = fixedSettings.FieldWidths;
        }
    }
}
