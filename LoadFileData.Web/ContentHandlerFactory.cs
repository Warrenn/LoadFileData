using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentHandlers.Settings;
using LoadFileData.Converters;
using Newtonsoft.Json;

namespace LoadFileData.Web
{
    public class ContentHandlerFactory : IContentHandlerFactory
    {
        private static IContentHandler Regex(
            RegexSettings settings,
            IDictionary<string, string> expressions)
        {
            foreach (var pair in expressions)
            {
                settings.FieldExpressions[pair.Key] = pair.Value;
            }
            return new RegexContentHandler(settings);
        }

        private static IContentHandler Indicies(
            FixedIndexSettings settings,
            IDictionary<string, int> indicies)
        {
            foreach (var pair in indicies)
            {
                settings.FieldIndices[pair.Value] = pair.Key;
            }
            return new FixedIndexContentHandler(settings);
        }

        private class CreateMethods
        {
            public Func<Type, ContentHandlerSettings> CreateSettings { get; set; }
            public Func<JsonReader, JsonSerializer, object> CreateDictionary { get; set; }
            public Func<ContentHandlerSettings, object, IContentHandler> CreateHandler { get; set; }
        }

        private static readonly Lazy<IDictionary<string, CreateMethods>> LazyFactories = new Lazy
            <IDictionary<string, CreateMethods>>(
            () =>
                (from info in typeof(ContentHandlerFactory).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                 let parameters = info.GetParameters()
                 where
                     (info.IsStatic &&
                      info.ReturnType == typeof(IContentHandler) &&
                      parameters.Count() == 2)
                 select new
                 {
                     info,
                     parameters
                 }).ToDictionary(m => m.info.Name, m => new CreateMethods
                    {
                        CreateDictionary =
                            (reader, serializer) => serializer.Deserialize(reader, m.parameters[1].ParameterType),
                        CreateHandler = (settings, o) => (IContentHandler)m.info.Invoke(null, new[] { settings, o }),
                        CreateSettings =
                            type => (ContentHandlerSettings)Activator.CreateInstance(m.parameters[0].ParameterType, type)
                    }, StringComparer.InvariantCultureIgnoreCase)
            );

        private readonly IDictionary<string, Type> typeMap;

        public ContentHandlerFactory(ITypeMapFactory typeMapFactory)
        {
            typeMap = typeMapFactory.CreateTypeMap();
        }
        #region Implementation of IFileHandlerFactory

        public IContentHandler Create(string jsonData)
        {
            Type type = null;
            CreateMethods factory = null;
            object dictionary = null;
            var properties = new Dictionary<string, object>();
            var conversions = new Dictionary<string, Func<object, object>>();
            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(jsonData))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    var key = jsonReader.Value as string;
                    if ((jsonReader.TokenType != JsonToken.PropertyName) || (string.IsNullOrEmpty(key)))
                    {
                        continue;
                    }
                    jsonReader.Read();
                    if (LazyFactories.Value.ContainsKey(key))
                    {
                        factory = LazyFactories.Value[key];
                        dictionary = factory.CreateDictionary(jsonReader, serializer);
                        continue;
                    }
                    if (typeMap.ContainsKey(key))
                    {
                        type = typeMap[key];
                        var conversion = serializer.Deserialize<IDictionary<string, string>>(jsonReader);
                        foreach (var pair in conversion)
                        {
                            var converter = ConverterManager.GetConverter(pair.Value);
                            if (converter == null)
                            {
                                continue;
                            }
                            conversions[pair.Key] = converter.Function;
                        }
                        continue;
                    }
                    if (!string.Equals(key, "settings", StringComparison.InvariantCultureIgnoreCase))
                    {
                        serializer.Deserialize(jsonReader);
                        continue;
                    }
                    properties = serializer.Deserialize<Dictionary<string, object>>(jsonReader);
                }
            }

            if ((type == null) || (factory == null))
            {
                return null;
            }

            var settings = factory.CreateSettings(type);

            foreach (var pair in conversions)
            {
                settings.FieldConversion[pair.Key] = pair.Value;
            }

            foreach (var pair in properties)
            {
                var propertyInfo = settings
                    .GetType()
                    .GetProperty(pair.Key);
                if (propertyInfo == null)
                {
                    continue;
                }
                propertyInfo.SetValue(settings, Convert.ChangeType(pair.Value, propertyInfo.PropertyType));
            }
            return factory.CreateHandler(settings, dictionary);
        }

        #endregion
    }
}
