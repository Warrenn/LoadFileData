using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using LoadFileData.ETLLayer.Constants;
using LoadFileData.ETLLayer.ContentReader.Settings;

namespace LoadFileData.ETLLayer.ContentReader
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

            try
            {
                Reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Known bug with excel reader sometimes will throw argument out of range exception http://exceldatareader.codeplex.com/discussions/431882
                var contents = ((MemoryStream) (fileStream)).ToArray();
                TempFileName = Path.GetTempFileName();
                File.WriteAllBytes(TempFileName, contents);
                TempFileStream = new FileStream(TempFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Reader = ExcelReaderFactory.CreateBinaryReader(TempFileStream);
            }
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
