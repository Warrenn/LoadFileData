using System.Collections.Generic;
using LoadFileData.ContentHandler.Settings;

namespace LoadFileData.ContentHandler
{
    public class FixedIndexContentHandler<T> : ContentHandlerBase<T> where T : new()
    {
        private readonly IDictionary<int, string> fieldIndices;

        public FixedIndexContentHandler(FixedIndexSettings<T> settings)
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
