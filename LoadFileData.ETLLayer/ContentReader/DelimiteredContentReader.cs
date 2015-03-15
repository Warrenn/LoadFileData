using System;
using LoadFileData.ETLLayer.ContentReader.Settings;
using Microsoft.VisualBasic.FileIO;

namespace LoadFileData.ETLLayer.ContentReader
{
    public class DelimiteredContentReader : CsvContentReaderBase
    {
        public override void ApplySettings(TextFieldParser parser, CsvSettings settings)
        {
            var delmitedSettings = (DelimitedSettings)settings;
            parser.TextFieldType = FieldType.FixedWidth;
            parser.Delimiters = delmitedSettings.Delimiters;
        }
    }
}
