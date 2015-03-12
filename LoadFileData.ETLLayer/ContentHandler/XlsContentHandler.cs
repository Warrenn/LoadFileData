using System;
using System.Collections.Generic;
using System.IO;
using LoadFileData.ETLLayer.ContentReader;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class XlsContentHandler : IContentReader, IExcelSettings , IRegexSettings
    {
        #region IExcelSettings Members

        public string SheetName
        {
            set { throw new NotImplementedException(); }
        }

        public int SheetNumber
        {
            set { throw new NotImplementedException(); }
        }

        public int HeaderLineNumber
        {
            set { throw new NotImplementedException(); }
        }

        public int ContentLineNumber
        {
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region IRegexSettings Members

        public void SetFieldRegexMapping(string fieldName, string regexPattern, Action<object, object> conversion = null)
        {
            throw new NotImplementedException();
        }

        #endregion

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
    }
}
