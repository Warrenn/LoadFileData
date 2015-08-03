using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public class ContentHandlerSettings
    {
        public ContentHandlerSettings(Type type)
        {
            FieldConversion = FieldConversionFactory.CreateDefault(type);
        }
        public IDictionary<string, Func<object, object>> FieldConversion { get; set; }
        public int ContentLineNumber { get; set; }
    }
}