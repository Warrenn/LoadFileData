using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadFileData.ContentHandler.Settings
{
    public class FixedIndexSettings<T>
    {
        public FixedIndexSettings()
        {
            ContentLineNumber = 1;
            FieldIndex = new List<string>(typeof (T).GetFields().Select(f => f.Name));
        }
        public IDictionary<string, Func<T, IDictionary<string, object>, T>> FieldConversion { get; set; }
        public IList<string> FieldIndex { get; set; }
        public int ContentLineNumber { get; set; }
    }
}
