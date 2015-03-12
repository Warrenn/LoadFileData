using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using LoadFileData.ETLLayer.Constants;
using LoadFileData.ETLLayer.ContentReader;
using Microsoft.VisualBasic.FileIO;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class FixedWidthContentHandler : IContentReader, ICsvSettings
    {
        protected readonly IDictionary<string, int> fieldWidths = new SortedDictionary<string, int>();
        protected string[] commentTokens = { };
        protected bool trimWhiteSpace = true;
        protected TextFieldParser parser;
        protected readonly IDictionary<string, Func<object, object>> fieldConversions =
            new SortedDictionary<string, Func<object, object>>();

        public virtual void SetFieldNameWidth(string fieldName, int width, Func<object, object> conversion = null)
        {
            if (conversion == null)
            {
                conversion = o => o;
            }

            fieldWidths[fieldName] = width;
            fieldConversions[fieldName] = conversion;

            foreach (var fieldWidth in fieldWidths.Where(kv => kv.Value == width).ToArray())
            {
                fieldWidths.Remove(fieldWidth);
                fieldConversions.Remove(fieldWidth.Key);
            }
        }

        #region IContentReader Members

        public virtual IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            parser = new TextFieldParser(fileStream)
            {
                TrimWhiteSpace = trimWhiteSpace,
                TextFieldType = FieldType.FixedWidth
            };
            if ((commentTokens != null) && (commentTokens.Length > 0))
            {
                parser.CommentTokens = commentTokens;
            }
            var headerList = fieldWidths
                .Where(kv => kv.Value > -1)
                .OrderBy(kv => kv.Value)
                .Select(kv => kv.Key).ToList();
            var lastHeader = fieldWidths.FirstOrDefault(kv => kv.Value == -1);
            if (lastHeader.Key != null)
            {
                headerList.Add(lastHeader.Key);
            }

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null)
                {
                    yield break;
                }

                var values = new SortedDictionary<string, object>();
                for (var index = 0; index < fields.Length; index++)
                {
                    var header = headerList[index];
                    var value = fieldConversions.ContainsKey(header)
                        ? fieldConversions[header](fields[index])
                        : fields[index];
                    values.Add(header, value);
                }
                yield return values;
            }
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            parser.Dispose(PolicyName.Disposable);
        }

        #endregion

        #region ICsvSettings Members

        public virtual string[] CommentTokens
        {
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(value.Length < 1);
                commentTokens = value;
            }
        }

        public virtual bool TrimWhiteSpace
        {
            set { trimWhiteSpace = value; }
        }

        #endregion
    }
}
