using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ContentReaders.Settings;

namespace LoadFileData.ContentReaders
{
    public class DelimiteredReader : TextReaderBase
    {
        private readonly string regexPattern;
        private readonly char[] trimChars;
        private readonly string[] commentStrings;

        public DelimiteredReader(DelimitedSettings settings)
            : base(settings)
        {
            regexPattern = CreateRegexPattern(settings.Delimiters, settings.CommentStrings);
            commentStrings = settings.CommentStrings;
            trimChars = settings.RemoveWhiteSpace ? GetTrimChars(settings.CommentStrings) : null;
        }

        public static string[] SplitLine(string line, params string[] delimiters)
        {
            return Split(line, delimiterStrings: delimiters, commentStrings: new[] {"\"", "'"});
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
            var trimChars = trim ? GetTrimChars(commentStrings) : null;
            var regexPattern = CreateRegexPattern(delimiterStrings, commentStrings);
            return Split(line, regexPattern, trimChars, commentStrings);
        }


        private static string RemoveInnerComments(string value, IReadOnlyCollection<string> commentStrings)
        {
            if (commentStrings == null || commentStrings.Count == 0)
            {
                return value;
            }
            value = commentStrings.Aggregate(value,
                ((seed, commentString) => seed.Replace(commentString + commentString, commentString)));
            return value;
        }

        private static char[] GetTrimChars(IReadOnlyCollection<string> commentStrings)
        {
            if (commentStrings == null || commentStrings.Count == 0)
            {
                return new char[] { };
            }
            var result =
                (from s in commentStrings
                 from c in s
                 select c).ToArray();
            return result;
        }

        private static string[] Split(string line, string regexPattern, char[] trimChars, IReadOnlyCollection<string> commentStrings)
        {
            var result = Regex.Split(line, regexPattern, RegexOptions.Compiled | RegexOptions.Singleline);
            return result
                .Select(s => RemoveInnerComments(s, commentStrings))
                .Select(s => trimChars == null ? s : s.Trim().Trim(trimChars)).ToArray();
        }

        public override IEnumerable<string> ReadRowValues(string line)
        {
            return Split(line, regexPattern, trimChars, commentStrings);
        }
    }
}
