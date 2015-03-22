using System.Collections.Generic;

namespace LoadFileData.ContentReader.Settings
{
    public class FolderSettings
    {
        public string Tenant { get; set; }
        public string FolderName { get; set; }
        public IDictionary<string, string> DefaultValues { get; set; }
        public IDictionary<string, ContentReaderSettings> ContentHandlers { get; set; }
    }
}
