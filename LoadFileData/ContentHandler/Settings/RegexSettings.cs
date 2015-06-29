using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.ContentHandler.Settings
{
    public class RegexSettings<T> : ContentHandlerSettings<T>
    {
        public RegexSettings()
        {
            FieldExpressions = typeof(T)
                .GetFields()
                .ToDictionary(f => f.Name, f => f.Name);
            HeaderLineNumber = 1;
            ContentLineNumber = 2;
        }
        public int HeaderLineNumber { get; set; }
        public IDictionary<string, string> FieldExpressions { get; set; }
    }
}
