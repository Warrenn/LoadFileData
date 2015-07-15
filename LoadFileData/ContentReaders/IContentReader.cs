using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ContentReaders
{
    public interface IContentReader
    {
        IEnumerable<IEnumerable<object>> ReadContent(Stream fileStream);

        int RowCount(Stream fileStream);
    }
}