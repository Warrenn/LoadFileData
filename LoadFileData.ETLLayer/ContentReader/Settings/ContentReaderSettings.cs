using System.Collections.Generic;

namespace LoadFileData.ETLLayer.ContentReader.Settings
{
    public class ContentReaderSettings
    {
        public virtual int HeaderLineNumber { get; set; }
        public virtual int ContentLineNumber { get; set; }
        public virtual IDictionary<string, FieldSettings> Fields { get; set; }

    }
}
