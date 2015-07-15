using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ContentHandlers.Settings;

namespace LoadFileData.ContentHandlers
{
    public class RegexContentHandler<T> : ContentHandlerBase<T> where T : new()
    {

        private readonly IDictionary<string, string> fieldExpressions;
        private readonly int headerLineNumber;

        public RegexContentHandler(RegexSettings<T> settings) : base(settings)
        {
            fieldExpressions = settings.FieldExpressions;
            headerLineNumber = settings.HeaderLineNumber;
        }
        
        public override IDictionary<int, string> GetFieldLookup(int lineNumber, object[] values)
        {
            if (lineNumber != headerLineNumber)
            {
                return null;
            }

            var lookup = new Dictionary<int, string>();
            for (var i = 0; i < values.Length; i++)
            {
                var value = string.Format("{0}", values[i]);
                foreach (var expression in fieldExpressions.Where(expression => Regex.IsMatch(value, expression.Value,
                    RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase |
                    RegexOptions.Singleline)))
                {
                    lookup[i] = expression.Key;
                }
            }
            return lookup;
        }
    }
}
