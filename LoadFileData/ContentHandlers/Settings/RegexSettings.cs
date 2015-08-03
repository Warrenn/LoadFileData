using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadFileData.ContentHandlers.Settings
{
    public class RegexSettings : ContentHandlerSettings
    {
        public RegexSettings(Type type)
            : base(type)
        {
            FieldExpressions = type
                .GetFields()
                .ToDictionary(f => f.Name, f => f.Name);
            HeaderLineNumber = 1;
            ContentLineNumber = 2;
        }
        public int HeaderLineNumber { get; set; }
        public IDictionary<string, string> FieldExpressions { get; set; }
    }
}
