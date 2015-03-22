using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using LoadFileData.Constants;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
{
    public class XlsContentReader : ContentReaderBase
    {
        protected IExcelDataReader Reader;
        protected string TempFileName = string.Empty;
        protected Stream TempFileStream;
        
        #region IContentReader Members

        public override IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream)
        {
            var settings = (ExcelSettings)Settings;
            var sheetName = settings.SheetName;
            var sheetNumber = settings.SheetNumber;

            Reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            var dataSource = Reader.AsDataSet();
            if (dataSource.Tables.Count < 1)
            {
                yield break;
            }
            var sheetIndex = ((sheetNumber > 0) && (sheetNumber <= dataSource.Tables.Count)) ? sheetNumber - 1  : 0;
            var dataTable = (string.IsNullOrEmpty(sheetName))
                ? dataSource.Tables[sheetIndex]
                : dataSource.Tables[sheetName];
            if (dataTable.Rows.Count < 1)
            {
                yield break;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                yield return row.ItemArray;
            }

        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Reader.Dispose(PolicyName.Disposable);
            TempFileStream.Dispose(PolicyName.Disposable);
            if ((!string.IsNullOrEmpty(TempFileName)) && (File.Exists(TempFileName)))
            {
                File.Delete(TempFileName);
            }
        }

        #endregion

    }
}
