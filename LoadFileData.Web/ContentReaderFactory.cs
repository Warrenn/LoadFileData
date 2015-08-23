using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LoadFileData.ContentReaders;
using LoadFileData.ContentReaders.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoadFileData.Web
{
    public class ContentReaderFactory : IContentReaderFactory
    {
        private static readonly IDictionary<string, Func<dynamic, IContentReader>> Factories =
            new SortedDictionary<string, Func<dynamic, IContentReader>>(
                StringComparer.InvariantCultureIgnoreCase)
            {
                {
                    "xlsx", dict =>
                        new XlsxContentReader(new ExcelSettings(dict.Range, dict.Sheet))
                },
                {
                    "xls", dict =>
                        new XlsContentReader(new ExcelSettings(dict.Range, dict.Sheet))
                },
                {
                    "fixed", dict =>
                        new FixedWidthReader(new FixedWidthSettings
                        {
                            FieldWidths = Array<int>(dict.Widths, 0),
                            RemoveWhiteSpace = dict.RemoveWhiteSpace ?? true
                        })
                },
                {
                    "delimitered", dict =>
                        new DelimiteredReader(new DelimitedSettings
                        {
                            CommentStrings = Array<string>(dict.CommentStrings, "\"", "'"),
                            Delimiters = Array<string>(dict.Delimiters, ",", "|"),
                            RemoveWhiteSpace = dict.RemoveWhiteSpace ?? true
                        })
                }
            };

        private static T[] Array<T>(JArray dataArray, params T[] defaultValue)
        {
            return (dataArray == null)
                ? defaultValue
                : (dataArray).Select(t => t.Value<T>()).ToArray();
        }

        #region Implementation of IContentReaderFactory

        public IContentReader Create(string jsonData)
        {
            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(jsonData))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    var key = jsonReader.Value as string;
                    if ((jsonReader.TokenType != JsonToken.PropertyName) ||
                        (string.IsNullOrEmpty(key)) ||
                        (!Factories.ContainsKey(key)))
                    {
                        continue;
                    }
                    jsonReader.Read();
                    dynamic settings = serializer.Deserialize(jsonReader);
                    return Factories[key](settings);
                }
            }
            return null;
        }

        #endregion
    }
}
