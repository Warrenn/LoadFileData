using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.ContentHandlers.Settings;

namespace LoadFileData.ContentHandlers
{
    public abstract class ContentHandlerBase : IContentHandler
    {
        private readonly IDictionary<string, Func<object, object>> converters;
        private readonly int contentLineNumber;
        private readonly Type type;

        protected ContentHandlerBase(ContentHandlerSettings settings)
        {
            type = settings.Type;
            converters = settings.FieldConversion;
            contentLineNumber = settings.ContentLineNumber;
        }

        public abstract IDictionary<int, string> GetFieldLookup(int lineNumber, object[] values);

        public virtual object Convert(IDictionary<string, object> keyValues)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var property in keyValues)
            {
                var value = property.Value;
                if (converters.ContainsKey(property.Key))
                {
                    value = converters[property.Key](value);
                }
                DynamicProperties.SetValue(instance, property.Key, value);
            }
            return instance;
        }

        public virtual IEnumerable<object> HandleContent(ContentHandlerContext context)
        {
            var lineNumber = 1;
            IDictionary<int, string> fieldLookup = new Dictionary<int, string>();
            foreach (var content in context.Content.Select(contentLine => contentLine.ToArray()))
            {
                var keyValues = new Dictionary<string, object>();
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