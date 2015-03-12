using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ETLLayer.FileReader
{
    public interface IContentReader : IDisposable
    {
        IEnumerable<IEnumerable<object>> RowData(Stream fileStream);
    }
}