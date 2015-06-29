using System.Collections.Generic;
using System.IO;
using LoadFileData.ContentReader.Settings;
using OfficeOpenXml;

namespace LoadFileData.ContentReader
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
            var package = new ExcelPackage(fileStream);
            var workbook = package.Workbook;

            if (workbook.Worksheets.Count < 1)
            {
                yield break;
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

            if (worksheet == null)
            {
                worksheet = workbook.Worksheets[1];
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
                    rowValues.Add(worksheet.Cells[rowNo, colNo]);
                }
                yield return rowValues;
            }
        }
    }
}