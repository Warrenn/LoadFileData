using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.ETLLayer.ContentReader;

namespace LoadFileData.ETLLayer.ContentHandler
{
    public class DelimiteredContentHandler : ICsvSettings, IContentReader, IRegexSettings
    {
        private string[] delimiters;

        #region IContentReader Members

        public IEnumerable<IDictionary<string, object>> RowData(Stream fileStream)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICsvSettings Members

        public string[] CommentTokens
        {
            set { throw new NotImplementedException(); }
        }

        public bool TrimWhiteSpace
        {
            set { throw new NotImplementedException(); }
        }

        public string[] Delimiters
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (value.Length < 1)
                {
                    throw new ArgumentNullException();
                }
                delimiters = value;
            }
        }

        #endregion

        #region IRegexSettings Members

        public void SetFieldRegexMapping(string fieldName, string regexPattern, Action<object, object> conversion = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
