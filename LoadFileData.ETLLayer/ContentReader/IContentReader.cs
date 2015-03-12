using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IContentReader : IDisposable
    {
        IEnumerable<IEnumerable<object>> RowData(Stream fileStream);
    }
}