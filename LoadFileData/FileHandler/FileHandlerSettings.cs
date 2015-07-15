using LoadFileData.ContentHandler;
using LoadFileData.ContentReader;
using LoadFileData.DAL;

namespace LoadFileData.FileHandler
{
    public class FileHandlerSettings<T> where T : new()
    {
        public string DestinationPath { get; set; }
        public IDataService Service { get; set; };
        public IContentReader Reader{ get; set; };
        public IContentHandler<T> ContentHandler{ get; set; }
        public string Name { get; set; }
    }
}
