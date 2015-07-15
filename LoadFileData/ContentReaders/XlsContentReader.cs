using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using LoadFileData.ContentReaders.Settings;

namespace LoadFileData.ContentReaders
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
            var table = GetTable(fileStream);
            if (table == null)
            {
                yield break;
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

        private DataTable GetTable(Stream fileStream)
        {
            fileStream.Position = 0;
            var reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
            var dataSource = reader.AsDataSet();
            if (dataSource.Tables.Count < 1)
            {
                return null;
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

            return table ?? dataSource.Tables[0];
        }


        public int RowCount(Stream fileStream)
        {
            var range = settings.Range;
            var rowEnd = range.RowEnd;
            if (rowEnd == null)
            {
                var table = GetTable(fileStream);
                if (table == null)
                {
                    return 0;
                }
                rowEnd = table.Rows.Count;
            }
            var rowCount = rowEnd.Value - (range.RowStart - 1);
            return (rowCount < 0) ? 0 : rowCount;
        }
    }
}
