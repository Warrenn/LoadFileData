using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ETLLayer.ContentReader.Settings;

namespace LoadFileData.ETLLayer.ContentReader
{
    public abstract class ContentReaderBase : IContentReader
    {
        protected ContentReaderSettings Settings;
        public abstract IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream);

        public virtual IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {

            var rowNumber = 1;
            string[] headers = {};

            foreach (var data in ReadRowData(fileStream).Select(rowData => rowData as object[] ?? rowData.ToArray()))
            {
                if (headers.Length == 0)
                {
                    var index = 0;
                    headers = data.Select(d => string.Format("Column{0}", index++)).ToArray();
                }
                if (rowNumber == Settings.HeaderLineNumber)
                {
                    headers = GetHeaders(data);
                }
                yield return GetValues(headers, data);
                rowNumber++;
            }
        }

        public virtual string[] GetHeaders(IEnumerable<object> values)
        {
            var valuesArray = values.ToArray();
            var headers = new string[valuesArray.Length];

            for (var index = 0; index < valuesArray.Length; index++)
            {
                var value = valuesArray[index];
                var headerString = string.Format("{0}", value);
                if (string.IsNullOrEmpty(headerString))
                {
                    headerString = string.Format("Column{0}", index);
                }
                var match = Settings.Fields.FirstOrDefault(
                    p =>
                        Regex.IsMatch(headerString, p.Value.RegexPattern,
                            RegexOptions.Compiled |
                            RegexOptions.IgnoreCase |
                            RegexOptions.IgnorePatternWhitespace))
                    .Key;
                headers[index] = string.IsNullOrEmpty(match) ? headerString : match;
            }
            return headers;
        }

        public virtual IDictionary<string, object> GetValues(string[] headers, IEnumerable<object> values)
        {
            var returnDictionary = new SortedDictionary<string, object>();
            var valuesArray = values.ToArray();
            for (var index = 0; index < headers.Length; index++)
            {
                var header = headers[index];
                var value = Settings.Fields.ContainsKey(header)
                    ? Settings.Fields[header].Conversion(valuesArray[index])
                    : valuesArray[index];
                returnDictionary.Add(header, value);
            }
            return returnDictionary;
        }

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
