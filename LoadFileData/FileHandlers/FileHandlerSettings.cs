using LoadFileData.ContentHandlers;
using LoadFileData.ContentReaders;
using LoadFileData.DAL;

namespace LoadFileData.FileHandlers
{
    public class FileHandlerSettings
    {
        public string DestinationPathTemplate { get; set; }
        public IServiceFactory ServiceFactory { get; set; }
        public IContentReader Reader { get; set; }
        public IContentHandler ContentHandler { get; set; }
        public IStreamManager StreamManager { get; set; }
        public string Name { get; set; }
    }
}
