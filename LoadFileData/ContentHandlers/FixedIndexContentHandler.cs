using System.Collections.Generic;
using System.Linq;
using LoadFileData.ContentHandlers.Settings;

namespace LoadFileData.ContentHandlers
{
    public class FixedIndexContentHandler : ContentHandlerBase
    {
        private readonly IDictionary<int, string> fieldIndices;

        public FixedIndexContentHandler(FixedIndexSettings settings)
            : base(settings)
        {
            fieldIndices = settings
                .FieldIndices
                .Where(i => i.Key > 0)
                .ToDictionary(i => i.Key - 1, i => i.Value);
        }

        public override IDictionary<int, string> GetFieldLookup(int lineNumber, object[] values)
        {
            return fieldIndices;
        }
    }
}
