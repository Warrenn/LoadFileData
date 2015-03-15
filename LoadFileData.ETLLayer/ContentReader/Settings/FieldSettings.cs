using System;

namespace LoadFileData.ETLLayer.ContentReader.Settings
{
    public class FieldSettings
    {
        public virtual string RegexPattern { get; set; }
        public virtual Func<object, object> Conversion { get; set; }
    }
}