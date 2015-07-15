using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public class ContentHandlerSettings<T>
    {
        public ContentHandlerSettings()
        {
            FieldConversion = FieldConversionFactory.CreateDefault<T>();
        }
        public IDictionary<string, Action<T, object>> FieldConversion { get; set; }
        public int ContentLineNumber { get; set; }
    }
}