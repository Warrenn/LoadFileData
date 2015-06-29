using System.Collections.Generic;

namespace LoadFileData.ContentHandler
{
    public class ContentHandlerContext
    {
        public string FileName { get; set; }
        public IEnumerable<IEnumerable<object>> Content { get; set; }
    }
}
