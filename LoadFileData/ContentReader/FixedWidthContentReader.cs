using System.Collections.Generic;
using System.IO;
using System.Linq;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
{
    public class FixedWidthContentReader : TextContentReader
    {

        public override IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream)
        {
            var fixedSettings = (FixedWidthSettings) Settings;
            var fixWidths = (fixedSettings.FieldWidths ?? new int[] {})
                .OrderBy(i => i)
                .Where(i => i > 0)
                .ToArray();

            return ReadRowLine(fileStream).Select(line => SubStrings(line, fixWidths));
        }

        public static IEnumerable<string> SubStrings(string line, int[] fieldWidths = null)
        {
            var index = 0;
            var stringLength = line.Length;
            var widths = ((fieldWidths == null) || (fieldWidths.Length == 0)) ? new[] {stringLength} : fieldWidths;
            foreach (var fieldWidth in widths)
            {
                var length = fieldWidth - index;
                if (fieldWidth >= stringLength)
                {
                    yield return line.Substring(index);
                    yield break;
                }
                yield return line.Substring(index, length);
                index = fieldWidth;
            }
        }
    }
}
