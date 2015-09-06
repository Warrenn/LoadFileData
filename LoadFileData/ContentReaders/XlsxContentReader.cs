using System.Collections.Generic;
using System.IO;
using LoadFileData.ContentReaders.Settings;
using OfficeOpenXml;

namespace LoadFileData.ContentReaders
{
    public class XlsxContentReader : IContentReader
    {
        private readonly ExcelSettings settings;

        public XlsxContentReader(ExcelSettings settings)
        {
            this.settings = settings;
        }

        public IEnumerable<IEnumerable<object>> ReadContent(Stream fileStream)
        {
            var worksheet = GetWorkSheet(fileStream);
            if (worksheet == null)
            {
                yield break;
            }

            var range = settings.Range;
            var dimension = worksheet.Dimension;

            var rowStartNo = range.RowStart;
            var colStartNo = range.ColumnStart;
            var rowEndNo = range.RowEnd ?? dimension.Rows;
            var colEndNo = range.ColumnEnd ?? dimension.Columns;
            if (colStartNo > colEndNo)
            {
                yield break;
            }
            if (rowStartNo > rowEndNo)
            {
                yield break;
            }

            for (var rowNo = rowStartNo; rowNo <= rowEndNo; rowNo++)
            {
                var rowValues = new List<object>();
                for (var colNo = colStartNo; colNo <= colEndNo; colNo++)
                {
                    rowValues.Add(worksheet.Cells[rowNo, colNo].Value);
                }
                yield return rowValues;
            }
        }

        private ExcelWorksheet GetWorkSheet(Stream fileStream)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            var package = new ExcelPackage(fileStream);
            var workbook = package.Workbook;

            if (workbook.Worksheets.Count < 1)
            {
                return null;
            }
            var sheetName = settings.Sheet;
            var worksheet = workbook.Worksheets[sheetName];

            int sheetNumber;
            if ((worksheet == null) &&
                (int.TryParse(sheetName, out sheetNumber)) &&
                (sheetNumber > 0) &&
                (sheetNumber <= workbook.Worksheets.Count))
            {
                worksheet = workbook.Worksheets[sheetNumber];
            }

            return worksheet ?? workbook.Worksheets[1];
        }


        public int RowCount(Stream fileStream)
        {
            var range = settings.Range;
            var rowEnd = range.RowEnd;
            if (rowEnd == null)
            {
                var worksheet = GetWorkSheet(fileStream);
                if (worksheet == null)
                {
                    return 0;
                }

                rowEnd = worksheet.Dimension.Rows;
            }
            var rowCount = (rowEnd.Value) - (range.RowStart - 1);
            return (rowCount < 0) ? 0 : rowCount;
        }
    }
}