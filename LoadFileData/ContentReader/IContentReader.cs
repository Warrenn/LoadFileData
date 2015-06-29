using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ContentReader
{
    public interface IContentReader
    {
        IEnumerable<IEnumerable<object>> ReadContent(Stream fileStream);
    }
}