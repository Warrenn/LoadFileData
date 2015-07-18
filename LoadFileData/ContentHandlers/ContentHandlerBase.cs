using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.ContentHandlers.Settings;

namespace LoadFileData.ContentHandlers
{
    public abstract class ContentHandlerBase<T> : IContentHandler<T> where T : new()
    {
        private readonly IDictionary<string, Func<object, object>> converters;
        private readonly int contentLineNumber;

        protected ContentHandlerBase(ContentHandlerSettings<T> settings)
        {
            converters = settings.FieldConversion;
            contentLineNumber = settings.ContentLineNumber;
        }

        public abstract IDictionary<int, string> GetFieldLookup(int lineNumber, object[] values);

        public virtual T Convert(IDictionary<string, object> keyValues)
        {
            var instance = new T();
            var setter = new DynamicProperties(instance);
            foreach (var property in keyValues)
            {
                var value = property.Value;
                if (converters.ContainsKey(property.Key))
                {
                    value = converters[property.Key](value);
                }
                setter.TrySetValue(property.Key, value);
            }
            return instance;
        }

        public virtual IEnumerable<T> HandleContent(ContentHandlerContext context)
        {
            var lineNumber = 1;
            IDictionary<int, string> fieldLookup = new Dictionary<int, string>();
            var keyValues = new Dictionary<string, object>();
            foreach (var content in context.Content.Select(contentLine => contentLine.ToArray()))
            {
                fieldLookup = GetFieldLookup(lineNumber, content) ?? fieldLookup;
                if (lineNumber < contentLineNumber)
                {
                    lineNumber++;
                    continue;
                }

                for (var i = 0; i < content.Length; i++)
                {
                    var field = fieldLookup.ContainsKey(i) ? fieldLookup[i] : string.Format("Column{0}", i);
                    keyValues.Add(field, content[i]);
                }
                yield return Convert(keyValues);

                lineNumber++;
            }
        }
    }
}