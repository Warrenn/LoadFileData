using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadFileData.ContentHandlers.Settings
{
    public class FixedIndexSettings : ContentHandlerSettings
    {
        public FixedIndexSettings(Type type)
            : base(type)
        {
            var index = -1;
            FieldIndices = type
                .GetFields()
                .ToDictionary(f => index++, f => f.Name);
            ContentLineNumber = 1;
        }

        public IDictionary<int, string> FieldIndices { get; set; }
    }
}
