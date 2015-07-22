using LoadFileData.ContentHandlers;
using LoadFileData.ContentReaders;
using LoadFileData.DAL;

namespace LoadFileData.FileHandlers
{
    public class FileHandlerSettings<T> where T : new()
    {
        public string DestinationPathTemplate { get; set; }
        public IDataService Service { get; set; }
        public IContentReader Reader { get; set; }
        public IContentHandler<T> ContentHandler { get; set; }
        public IStreamManager StreamManager { get; set; }
        public string Name { get; set; }
    }
}
