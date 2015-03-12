using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LoadFileData.ETLLayer.ContentReader;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class FixedWidthContentHandler : IContentReader, ICsvSettings
    {
        private readonly IDictionary<string, int> fieldWidths = new SortedDictionary<string, int>();

        public void SetFieldNameWidth(string fieldName, int width, Action<object, object> conversion = null)
        {
            fieldWidths[fieldName] = width;
            foreach (var fieldWidth in fieldWidths.Where(kv => kv.Value == width).ToArray())
            {
                fieldWidths.Remove(fieldWidth);
            }
        }

        #region IContentReader Members

        public IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICsvSettings Members

        public string[] CommentTokens
        {
            set { throw new NotImplementedException(); }
        }

        public bool TrimWhiteSpace
        {
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}
