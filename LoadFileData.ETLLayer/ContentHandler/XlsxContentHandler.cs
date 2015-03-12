﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ETLLayer.Constants;
using LoadFileData.ETLLayer.ContentReader;
using OfficeOpenXml;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class XlsxContentHandler : IContentReader, IExcelSettings, IRegexSettings
    {
        protected ExcelPackage package;
        protected string[] fieldNames;
        protected ExcelWorkbook workbook;
        protected string sheetName;
        protected int sheetNumber = 1;
        protected int headerLineNumber = 1;
        protected string tempFileName = string.Empty;
        protected Stream tempFileStream;
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
            set
            {
                headerLineNumber = value;
                contentLineNumber = value + 1;
            }
        }

        public virtual int ContentLineNumber
        {
            set { contentLineNumber = value; }
        }

        #endregion

        #region IRegexSettings Members

        public virtual void SetFieldRegexMapping(string fieldName, string regexPattern,
            Func<object, object> conversion = null)
        {
            if (conversion == null)
            {
                conversion = o => o;
            }

            headerRegExPatterns[fieldName] = regexPattern;
            fieldConversions[fieldName] = conversion;
        }

        #endregion

        #region IContentReader Members

        public virtual IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            package = new ExcelPackage(fileStream);
            workbook = package.Workbook;

            sheetNumber = ((sheetNumber > 0) && (sheetNumber <= workbook.Worksheets.Count)) ? sheetNumber : 1;
            var worksheet = (string.IsNullOrEmpty(sheetName))
                ? workbook.Worksheets[sheetNumber]
                : workbook.Worksheets[sheetName];

            if (dataTable.Rows.Count < 1)
            {
                yield break;
            }
            var headerRowIndex = (headerLineNumber > dataTable.Rows.Count || headerLineNumber < 1)
                ? dataTable.Rows.Count - 1
                : headerLineNumber - 1;
            var headerRow = dataTable.Rows[headerRowIndex];

            var headerList = (
                from header in headerRow.ItemArray.Select(i => i.ToString())
                let match =
                    headerRegExPatterns.FirstOrDefault(
                        p =>
                            Regex.IsMatch(header, p.Value,
                                RegexOptions.Compiled |
                                RegexOptions.IgnoreCase |
                                RegexOptions.IgnorePatternWhitespace))
                        .Key
                select !string.IsNullOrEmpty(match) ? match : header).ToList();

            var contentRowIndex = (contentLineNumber > dataTable.Rows.Count || contentLineNumber < 1)
                ? dataTable.Rows.Count - 1
                : contentLineNumber - 1;

            for (var rowIndex = contentRowIndex; rowIndex < dataTable.Rows.Count; rowIndex++)
            {
                var fields = dataTable.Rows[rowIndex].ItemArray;

                var values = new SortedDictionary<string, object>();
                for (var index = 0; index < fields.Length; index++)
                {
                    var header = headerList[index];
                    var value = fieldConversions.ContainsKey(header)
                        ? fieldConversions[header](fields[index])
                        : fields[index];
                    values.Add(header, value);
                }
                yield return values;
            }

        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            Package.Dispose(PolicyName.Disposable);
        }

        #endregion    
    }
}