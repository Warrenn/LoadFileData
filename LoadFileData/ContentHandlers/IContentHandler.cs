using System.Collections.Generic;
using LoadFileData.ContentHandler;

namespace LoadFileData.ContentHandlers
{
    public interface IContentHandler<out T> where T : new()
    {
        IEnumerable<T> HandleContent(ContentHandlerContext context);
    }
}
