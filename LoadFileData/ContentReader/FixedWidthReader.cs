using System.Collections.Generic;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
{
    public class FixedWidthReader : TextReaderBase
    {
        private readonly int[] fieldWidths;
        private readonly bool removeWhitespace;

        public FixedWidthReader(FixedWidthSettings settings) : base(settings)
        {
            removeWhitespace = settings.RemoveWhiteSpace;
            fieldWidths = settings.FieldWidths;
        }

        public override IEnumerable<string> ReadRowValues(string line)
        {
            var index = 0;
            var stringLength = line.Length;
            var widths = ((fieldWidths == null) || (fieldWidths.Length == 0)) ? new[] { stringLength } : fieldWidths;
            foreach (var fieldWidth in widths)
            {
                var length = fieldWidth - index;
                if (fieldWidth >= stringLength)
                {
                    yield return removeWhitespace
                        ? line.Substring(index).Trim()
                        : line.Substring(index);
                    yield break;
                }
                yield return removeWhitespace
                    ? line.Substring(index, length).Trim()
                    : line.Substring(index, length);
                index = fieldWidth;
            }
        }
    }
}
