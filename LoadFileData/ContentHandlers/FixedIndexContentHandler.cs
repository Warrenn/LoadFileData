using System.Collections.Generic;
using LoadFileData.ContentHandlers.Settings;

namespace LoadFileData.ContentHandlers
{
    public class FixedIndexContentHandler : ContentHandlerBase
    {
        private readonly IDictionary<int, string> fieldIndices;

        public FixedIndexContentHandler(FixedIndexSettings settings)
            : base(settings)
        {
            fieldIndices = settings.FieldIndices;
        }

        public override IDictionary<int, string> GetFieldLookup(int lineNumber, object[] values)
        {
            return fieldIndices;
        }
    }
}
