using System.Collections.Generic;
using System.IO;
using LoadFileData.ETLLayer.Constants;
using LoadFileData.ETLLayer.ContentReader.Settings;
using OfficeOpenXml;

namespace LoadFileData.ETLLayer.ContentReader
{
    public class XlsxContentReader : ContentReaderBase
    {
        protected ExcelPackage Package;
        protected ExcelWorkbook Workbook;
        protected string TempFileName = string.Empty;
        protected Stream TempFileStream;

        #region IContentReader Members

        public override IEnumerable<IEnumerable<object>> ReadRowData(Stream fileStream)
        {
            var settings = (ExcelSettings)Settings;
            var sheetName = settings.SheetName;
            var sheetNumber = settings.SheetNumber;

            Package = new ExcelPackage(fileStream);
            Workbook = Package.Workbook;

            sheetNumber = ((sheetNumber > 0) && (sheetNumber <= Workbook.Worksheets.Count)) ? sheetNumber : 1;
            var worksheet = (string.IsNullOrEmpty(sheetName))
                ? Workbook.Worksheets[sheetNumber]
                : Workbook.Worksheets[sheetName];
            var dimension = worksheet.Dimension;
            if (dimension.Rows < 1)
            {
                yield break;
            }

            for (var rowNumber = 1; rowNumber <= dimension.Rows; rowNumber++)
            {
                var rowValues = new List<object>();
                for (var columnNumber = 1; columnNumber <= dimension.Columns; columnNumber++)
                {
                    rowValues.Add(worksheet.Cells[rowNumber, columnNumber]);
                }
                yield return rowValues;
            }

        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Package.Dispose(PolicyName.Disposable);
        }

        #endregion    
    }
}