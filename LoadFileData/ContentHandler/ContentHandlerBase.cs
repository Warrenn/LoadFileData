using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.ContentHandler.Settings;

namespace LoadFileData.ContentHandler
{
    public abstract class ContentHandlerBase<T> : IContentHandler<T> where T : new()
    {
        private readonly IDictionary<string, Action<T, object>> converters;
        private readonly int contentLineNumber;

        protected ContentHandlerBase(ContentHandlerSettings<T> settings)
        {
            converters = settings.FieldConversion;
            contentLineNumber = settings.ContentLineNumber;
        }

        public abstract IDictionary<int, string> GetFieldLookup(int lineNumber, object[] values);

        public IEnumerable<T> HandleContent(ContentHandlerContext context)
        {
            var lineNumber = 1;
            IDictionary<int, string> fieldLookup = new Dictionary<int, string>();
            foreach (var content in context.Content.Select(contentLine => contentLine.ToArray()))
            {
                fieldLookup = GetFieldLookup(lineNumber, content) ?? fieldLookup;
                if (lineNumber < contentLineNumber)
                {
                    lineNumber++;
                    continue;
                }
                var instance = new T();
                for (var i = 0; i < content.Length; i++)
                {
                    var field = fieldLookup.ContainsKey(i) ? fieldLookup[i] : string.Format("Column{0}", i);
                    
                    if (converters.ContainsKey(field))
                    {
                        converters[field](instance, content[i]);
                    }
                }
                yield return instance;

                lineNumber++;
            }
        }
    }
}