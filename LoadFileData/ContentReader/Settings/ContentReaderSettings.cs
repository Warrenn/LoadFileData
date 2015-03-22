using System.Collections.Generic;

namespace LoadFileData.ContentReader.Settings
{
    public class ContentReaderSettings
    {
        public ContentReaderSettings()
        {
            HeaderLineNumber = 1;
            Fields = new Dictionary<string, FieldSettings>();
        }
        public int HeaderLineNumber { get; set; }
        public IDictionary<string, FieldSettings> Fields { get; set; }

    }
}
