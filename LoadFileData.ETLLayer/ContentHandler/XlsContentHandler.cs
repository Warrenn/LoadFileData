using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using LoadFileData.ETLLayer.ContentReader;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class XlsContentHandler : IContentReader, IExcelSettings , IRegexSettings
    {
        protected IExcelDataReader reader;
        protected string sheetName;
        protected int sheetNumber = 1;
        protected int headerLineNumber = 1;
        protected int contentLineNumber = 2;
        protected readonly IDictionary<string, Func<object, object>> fieldConversions =
            new SortedDictionary<string, Func<object, object>>();

        protected readonly IDictionary<string, string> headerRegExPatterns =
            new SortedDictionary<string, string>();


        #region IExcelSettings Members

        public virtual string SheetName
        {
            set { sheetName = value; }
        }

        public virtual int SheetNumber
        {
            set { sheetNumber = value; }
        }

        public virtual int HeaderLineNumber
        {
            set { headerLineNumber = value; }
        }

        public virtual int ContentLineNumber
        {
            set { contentLineNumber = value; }
        }

        #endregion

        #region IRegexSettings Members

        public virtual void SetFieldRegexMapping(string fieldName, string regexPattern, Func<object, object> conversion = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IContentReader Members

        public virtual IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            ExcelReaderFactory.CreateBinaryReader()
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
