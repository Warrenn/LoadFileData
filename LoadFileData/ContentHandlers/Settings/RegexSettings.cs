using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadFileData.ContentHandlers.Settings
{
    public class RegexSettings : ContentHandlerSettings
    {
        public int HeaderLineNumber { get; set; }
        public IDictionary<string, string> FieldExpressions { get; set; }
        public RegexSettings(Type type)
            : base(type)
        {
            FieldExpressions = type
                .GetProperties()
                .ToDictionary(f => f.Name, f => f.Name, StringComparer.InvariantCultureIgnoreCase);
            HeaderLineNumber = 1;
            ContentLineNumber = 2;
        }
    }
}
