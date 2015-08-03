using System.Collections.Generic;

namespace LoadFileData.ContentHandlers
{
    public interface IContentHandler
    {
        IEnumerable<object> HandleContent(ContentHandlerContext context);
    }
}
