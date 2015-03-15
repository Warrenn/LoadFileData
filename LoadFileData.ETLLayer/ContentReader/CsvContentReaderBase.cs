using System.Collections.Generic;
using System.IO;
using LoadFileData.ETLLayer.Constants;
using LoadFileData.ETLLayer.ContentReader.Settings;
using Microsoft.VisualBasic.FileIO;

namespace LoadFileData.ETLLayer.ContentReader
{
    public abstract class CsvContentReaderBase : ContentReaderBase
    {
        protected TextFieldParser Parser;

        public abstract void ApplySettings(TextFieldParser parser, CsvSettings settings);

        public override IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream)
        {
            var settings = (CsvSettings) Settings;

            Parser = new TextFieldParser(fileStream)
            {
                CommentTokens = settings.CommentTokens,
                TrimWhiteSpace = settings.TrimWhiteSpace,
                HasFieldsEnclosedInQuotes = settings.HasFieldsEnclosedInQuotes
            };
            ApplySettings(Parser, settings);
            while (!Parser.EndOfData)
            {
                yield return Parser.ReadFields();
            }
        }

        public override void Dispose()
        {
            Parser.Dispose(PolicyName.Disposable);
        }
    }
}
