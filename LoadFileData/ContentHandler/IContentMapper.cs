using System.Collections.Generic;

namespace LoadFileData.ContentHandler
{
    public interface IContentMapper<out T>
    {
        IEnumerable<T> MapContent(ContentMapperContext context);
    }
}
