using System.Collections.Generic;

namespace LoadFileData.ContentHandler
{
    public interface IContentHandler<out T> where T : new()
    {
        IEnumerable<T> HandleContent(ContentHandlerContext context);
    }
}
