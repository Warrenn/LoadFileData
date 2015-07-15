using System.Collections.Generic;
using LoadFileData.ContentHandler;
using LoadFileData.ContentHandlers.Settings;

namespace LoadFileData.ContentHandlers
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
