using System.Collections.Generic;

namespace LoadFileData.ContentHandlers
{
    public class ContentHandlerContext
    {
        public string FileName { get; set; }
        public IEnumerable<IEnumerable<object>> Content { get; set; }
    }
}
