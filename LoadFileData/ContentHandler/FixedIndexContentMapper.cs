using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.ContentHandler.Settings;

namespace LoadFileData.ContentHandler
{
    public class FixedIndexContentMapper<T> : IContentMapper<T>
    {
        private readonly FixedIndexSettings<T> settings;

        public FixedIndexContentMapper(FixedIndexSettings<T> settings)
        {
            this.settings = settings;
        }

        public IEnumerable<T> MapContent(ContentMapperContext context)
        {
            var lineNumber = 1;
            foreach (var contentLine in context.Content)
            {
                settings.Fields[]
            }
        }
    }
}
