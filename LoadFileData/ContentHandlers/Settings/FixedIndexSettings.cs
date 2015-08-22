using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadFileData.ContentHandlers.Settings
{
    public class FixedIndexSettings : ContentHandlerSettings
    {
        public IDictionary<int, string> FieldIndices { get; set; }

        public FixedIndexSettings(Type type)
            : base(type)
        {
            var index = 1;
            FieldIndices = type
                .GetProperties()
                .ToDictionary(f => index++, f => f.Name);
            ContentLineNumber = 1;
        }
    }
}
