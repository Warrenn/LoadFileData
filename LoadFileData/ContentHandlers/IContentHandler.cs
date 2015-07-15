using System.Collections.Generic;

namespace LoadFileData.ContentHandlers
{
    public interface IContentHandler<out T> where T : new()
    {
        IEnumerable<T> HandleContent(ContentHandlerContext context);
    }
}
