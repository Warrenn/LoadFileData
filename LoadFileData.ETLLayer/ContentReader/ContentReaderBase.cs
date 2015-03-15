using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ETLLayer.ContentReader.Settings;

namespace LoadFileData.ETLLayer.ContentReader
{
    public abstract class ContentReaderBase : IContentReader
    {
        protected string[] Headers;
        protected ContentReaderSettings Settings;
        public abstract IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream);

        public IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            var headerValues = ReadRowData(fileStream)
                .Skip(Settings.HeaderLineNumber - 1)
                .FirstOrDefault();
            if (headerValues == null)
            {
                yield break;
            }

            SetHeaders(headerValues);

            foreach (
                var rowData in ReadRowData(fileStream).Skip(Settings.ContentLineNumber - Settings.ContentLineNumber))
            {
                yield return GetValues(rowData);
            }
        }

        public void SetHeaders(IEnumerable<object> values)
        {
            Headers = (
                from header in values
                let headerString = string.Format("{0}", header)
                let match =
                    Settings.Fields.FirstOrDefault(
                        p =>
                            Regex.IsMatch(headerString, p.Value.RegexPattern,
                                RegexOptions.Compiled |
                                RegexOptions.IgnoreCase |
                                RegexOptions.IgnorePatternWhitespace))
                        .Key
                select !string.IsNullOrEmpty(match) ? match : headerString).ToArray();
        }

        public IDictionary<string, object> GetValues(IEnumerable<object> values)
        {
            var returnDictionary = new SortedDictionary<string, object>();
            var valuesArray = values.ToArray();
            for (var index = 0; index < Headers.Length; index++)
            {
                var header = Headers[index];
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
