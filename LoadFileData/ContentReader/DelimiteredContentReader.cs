using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
{
    public class DelimiteredContentReader : TextContentReader
    {

        public static string[] Split(
            string line,
            string[] delimiterStrings = null,
            string[] commentStrings = null,
            bool trim = true)
        {
            var delimiters = ((delimiterStrings == null) || (delimiterStrings.Length == 0))
                ? @"\|,"
                : string.Concat(delimiterStrings);

            var comments = ((commentStrings == null) || (commentStrings.Length == 0))
                ? @"""'"
                : string.Concat(commentStrings);

            var regexPattern = @"[" + delimiters + @"](?=(?:[^" + comments + @"]*[" + comments + @"][^" + comments +
                               @"]*[" + comments + @"])*(?![^" + comments + @"]*[" + comments + @"]))";
            var result = Regex.Split(line, regexPattern, RegexOptions.Compiled | RegexOptions.Singleline);
            return result.Select(s => trim ? s.Trim() : s).ToArray();
        }

        public override IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream)
        {
            var settings = (DelimitedSettings) Settings;

            return ReadRowLine(fileStream).Select(line => Split(line, settings.Delimiters, settings.CommentStrings));
        }
    }
}
