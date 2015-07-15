using System.Collections.Generic;
using System.Linq;

namespace LoadFileData.ContentHandlers.Settings
{
    public class FixedIndexSettings<T> : ContentHandlerSettings<T>
    {
        public FixedIndexSettings()
        {
            var index = -1;
            FieldIndices = typeof (T)
                .GetFields()
                .ToDictionary(f => index++, f => f.Name);
            ContentLineNumber = 1;
        }

        public IDictionary<int, string> FieldIndices { get; set; }
    }
}
