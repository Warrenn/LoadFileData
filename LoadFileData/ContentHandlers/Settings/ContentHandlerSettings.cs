using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public class ContentHandlerSettings
    {
        public Type Type { get; set; }
        public IDictionary<string, Func<object, object>> FieldConversion { get; set; }
        public int ContentLineNumber { get; set; }

        public ContentHandlerSettings()
        {
            Type = typeof (object);
            FieldConversion = new Dictionary<string, Func<object, object>>();
        }

        public ContentHandlerSettings(Type type)
        {
            Type = type;
            FieldConversion = FieldConversionFactory.CreateDefault(type);
        }
    }
}