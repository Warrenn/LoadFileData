using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
{
    public abstract class TextContentReader : ContentReaderBase
    {
        public IEnumerable<string> ReadRowLine(Stream fileStream)
        {
            var settings = (TextReaderSettings) Settings;
            var qouteStrings = settings.CommentStrings;

            using (var reader = new StreamReader(fileStream, Encoding.UTF8, true))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
s                    if ((string.IsNullOrEmpty(line)) ||
                        (qouteStrings == null) ||
                        (qouteStrings.Any(line.StartsWith)))
                    {
                        continue;
                    }
                    yield return (settings.RemoveWhiteSpace) ? line.Trim() : line;
                }
            }
        }
    }
}
