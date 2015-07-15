using System;
using System.Text.RegularExpressions;

namespace LoadFileData.ContentReaders.Settings
{
    public static class ExcelRangeFactory
    {
        private static Tuple<int?, int?> ColumnRow(string columRowString)
        {
            if (string.IsNullOrEmpty(columRowString))
            {
                return new Tuple<int?, int?>(null, null);
            }
            var columnRowMatch = Regex.Match(columRowString, "^([a-zA-Z]+|\\?)([1-9][0-9]*|\\?)$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

            if (!columnRowMatch.Success)
            {
                var parts = columRowString.Split(',');
                return parts.Length > 1
                    ? new Tuple<int?, int?>(GetInt(parts[0]), GetInt(parts[1]))
                    : new Tuple<int?, int?>(GetInt(parts[0]), null);
            }

            var columnString = columnRowMatch.Groups[1].Value.ToUpper();
            var rowString = columnRowMatch.Groups[2].Value;

            int? column = null;
            if (columnString != "?")
            {
                var powerOf = columnString.Length - 1;
                column = 0;
                for (var i = 0; i < columnString.Length; i++)
                {
                    column += (columnString[i] - '@')*(10 ^ (powerOf - i));
                }
                if (column < 1)
                {
                    column = 1;
                }
            }

            if (rowString == "?")
            {
                return new Tuple<int?, int?>(column, null);
            }

            int row;
            return int.TryParse(rowString, out row)
                ? new Tuple<int?, int?>(column, row)
                : new Tuple<int?, int?>(column, null);
        }

        private static int? GetInt(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }
            int outValue;
            if (!int.TryParse(val, out outValue))
            {
                return null;
            }
            return outValue < 1 ? 1 : outValue;
        }

        public static ExcelRange CreateRange(string range)
        {
            var returnValue = new ExcelRange();
            if (string.IsNullOrEmpty(range))
            {
                return returnValue;
            }
            var startEndParts = range.Split(':');
            var columnRowStart = ColumnRow(startEndParts[0]);
            if ((columnRowStart.Item1 == null) ||
                (columnRowStart.Item2 == null))
            {
                return returnValue;
            }
            returnValue.ColumnStart = (int) columnRowStart.Item1;
            returnValue.RowStart = (int) columnRowStart.Item2;
            if (startEndParts.Length <= 1)
            {
                return returnValue;
            }
            var columnRowEnd = ColumnRow(startEndParts[1]);
            returnValue.ColumnEnd = columnRowEnd.Item1;
            returnValue.RowEnd = columnRowEnd.Item2;
            return returnValue;
        }
    }
}