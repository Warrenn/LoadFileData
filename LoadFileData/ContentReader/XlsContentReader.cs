using System.Collections.Generic;
using System.IO;
using Excel;
using LoadFileData.ContentReader.Settings;

namespace LoadFileData.ContentReader
{
    public class XlsContentReader : IContentReader
    {
        private readonly ExcelSettings settings;

        public XlsContentReader(ExcelSettings settings)
        {
            this.settings = settings;
        }

        public IEnumerable<IEnumerable<object>> ReadContent(Stream fileStream)
        {
            var reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            var dataSource = reader.AsDataSet();
            if (dataSource.Tables.Count < 1)
            {
                yield break;
            }

            var sheetName = settings.Sheet;
            var table = dataSource.Tables[sheetName];

            int sheetNumber;
            if ((table == null) &&
                (int.TryParse(sheetName, out sheetNumber)) &&
                (sheetNumber > 0) &&
                (sheetNumber <= dataSource.Tables.Count))
            {
                table = dataSource.Tables[(sheetNumber - 1)];
            }

            if (table == null)
            {
                table = dataSource.Tables[0];
            }

            var range = settings.Range;

            var rowStartIndex = range.RowStart - 1;
            var colStartIndex = range.ColumnStart - 1;
            var rowEndIndex = (range.RowEnd ?? table.Rows.Count) - 1;
            var colEndIndex = (range.ColumnEnd ?? table.Columns.Count) - 1;
            if (colStartIndex > colEndIndex)
            {
                yield break;
            }
            if (rowStartIndex > rowEndIndex)
            {
                yield break;
            }

            for (var rowIndex = rowStartIndex; rowIndex < rowEndIndex; rowIndex++)
            {
                var returnArray = new object[(colEndIndex - colStartIndex) + 1];
                table.Rows[rowIndex].ItemArray.CopyTo(returnArray, colStartIndex);
                yield return returnArray;
            }
        }
    }
}
