using System.Collections.Generic;

namespace LoadFileData.ContentHandler
{
    public interface IContentHandler
    {
        void ProcessRowData(IDictionary<string, object> rowData);
    }
}
