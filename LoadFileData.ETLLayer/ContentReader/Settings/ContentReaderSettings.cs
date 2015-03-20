using System.Collections.Generic;

namespace LoadFileData.ETLLayer.ContentReader.Settings
{
    public class ContentReaderSettings
    {
        public ContentReaderSettings()
        {
            HeaderLineNumber = 1;
            Fields = new Dictionary<string, FieldSettings>();
        }
        public virtual int HeaderLineNumber { get; set; }
        public virtual IDictionary<string, FieldSettings> Fields { get; set; }

    }
}
