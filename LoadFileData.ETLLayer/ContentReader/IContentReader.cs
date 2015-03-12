using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IContentReader : IDisposable
    {
        IEnumerable<IDictionary<string, object>> RowData(Stream fileStream);
    }
}