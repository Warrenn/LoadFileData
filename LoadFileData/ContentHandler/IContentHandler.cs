using System.Collections.Generic;
using LoadFileData.DAL.Entry;

namespace LoadFileData.ContentHandler
{
    public interface IContentHandler
    {
        IEnumerable<DataEntry> ProcessRowData(IDictionary<string, object> rowData);
    }
}
