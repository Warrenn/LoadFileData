using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ContentReader;
using LoadFileData.ContentReaders.Settings;

namespace LoadFileData.ContentReaders
{
    public class DelimiteredReader : TextReaderBase
    {
        private readonly string regexPattern;
        private readonly bool removeWhiteSpace;

        public DelimiteredReader(DelimitedSettings settings) : base(settings)
        {
            regexPattern = CreateRegexPattern(settings.Delimiters, settings.CommentStrings);
            removeWhiteSpace = settings.RemoveWhiteSpace;
        }

        public static string[] Split(string line, params string[] delimiters)
        {
            return Split(line, delimiterStrings: delimiters);
        }

        private static string CreateRegexPattern(
            string[] delimiterStrings,
            string[] commentStrings)
        {
            var delimiters = ((delimiterStrings == null) || (delimiterStrings.Length == 0))
                ? @"\|,"
                : string.Concat(delimiterStrings);

            var comments = ((commentStrings == null) || (commentStrings.Length == 0))
                ? @"""'"
                : string.Concat(commentStrings);

            return @"[" + delimiters + @"](?=(?:[^" + comments + @"]*[" + comments + @"][^" + comments +
                   @"]*[" + comments + @"])*(?![^" + comments + @"]*[" + comments + @"]))";
        }

        public static string[] Split(
            string line,
            bool trim = true,
            string[] delimiterStrings = null,
            string[] commentStrings = null)
        {
            var regexPattern = CreateRegexPattern(delimiterStrings, commentStrings);
            return Split(line, trim, regexPattern);
        }

        private static string[] Split(string line, bool trim, string regexPattern)
        {
            var result = Regex.Split(line, regexPattern, RegexOptions.Compiled | RegexOptions.Singleline);
            return result.Select(s => trim ? s.Trim() : s).ToArray();
        }
        
        public override IEnumerable<string> ReadRowValues(string line)
        {
            return Split(line, removeWhiteSpace, regexPattern);
        }
    }
}
