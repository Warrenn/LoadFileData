using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LoadFileData.ETLLayer.Constants;
using LoadFileData.ETLLayer.ContentReader;
using Microsoft.VisualBasic.FileIO;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class DelimiteredContentHandler : ICsvSettings, IContentReader, IRegexSettings
    {
        protected string[] delimiters = {";", ","};
        protected string[] commentTokens = {};
        protected bool trimWhiteSpace = true;
        protected TextFieldParser parser;
        protected readonly IDictionary<string,Func<object,object>> fieldConversions = 
            new SortedDictionary<string, Func<object, object>>();

        protected readonly IDictionary<string, string> headerRegExPatterns =
            new SortedDictionary<string, string>();

        #region IContentReader Members

        public virtual IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            parser = new TextFieldParser(fileStream)
            {
                TrimWhiteSpace = trimWhiteSpace,
                TextFieldType = FieldType.Delimited
            };
            if ((commentTokens != null) && (commentTokens.Length > 0))
            {
                parser.CommentTokens = commentTokens;
            }
            if ((delimiters != null) && (delimiters.Length > 0))
            {
                parser.Delimiters = delimiters;
            }

            var headers = parser.ReadFields();
            if (headers == null)
            {
                yield break;
            }

            var headerList = (
                from header in headers
                let header1 = header
                let match =
                    headerRegExPatterns.FirstOrDefault(
                        p =>
                            Regex.IsMatch(header1, p.Value,
                                RegexOptions.Compiled |
                                RegexOptions.IgnoreCase |
                                RegexOptions.IgnorePatternWhitespace))
                        .Key
                select !string.IsNullOrEmpty(match) ? match : header).ToList();

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null)
                {
                    yield break;
                }
                
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
            parser.Dispose(PolicyName.Disposable);
        }

        #endregion

        #region ICsvSettings Members

        public virtual string[] CommentTokens
        {
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(value.Length < 1);
                commentTokens = value;
            }
        }

        public virtual bool TrimWhiteSpace
        {
            set { trimWhiteSpace = value; }
        }

        public virtual string[] Delimiters
        {
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(value.Length < 1);
                delimiters = value;
            }
        }

        #endregion

        #region IRegexSettings Members

        public virtual void SetFieldRegexMapping(string fieldName, string regexPattern, Func<object, object> conversion = null)
        {
            if (conversion == null)
            {
                conversion = o => o;
            }

            headerRegExPatterns[fieldName] = regexPattern;
            fieldConversions[fieldName] = conversion;
        }

        #endregion
    }
}
