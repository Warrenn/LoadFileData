using System;
using System.Collections.Generic;
using System.IO;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentHandlers.Settings;
using LoadFileData.Converters;
using Newtonsoft.Json;

namespace LoadFileData.Web
{
    public class ContentHandlerFactory : IContentHandlerFactory
    {
        private static readonly
            IDictionary
                <string,
                    Func<
                        JsonReader,
                        JsonSerializer,
                        ContentHandlerSettings,
                        IDictionary<string, object>,
                        ContentHandlerSettings>> Factories =
                            new Dictionary
                                <string,
                                    Func<
                                        JsonReader,
                                        JsonSerializer,
                                        ContentHandlerSettings,
                                        IDictionary<string, object>,
                                        ContentHandlerSettings>>(StringComparer.InvariantCultureIgnoreCase)
                            {
                                {
                                    "regex", (reader, serializer, handlerSettings, properties) =>
                                    {
                                        RegexSettings returnValue;
                                        var fieldExpressions =
                                            serializer.Deserialize<IDictionary<string, string>>(reader);
                                        if (handlerSettings != null)
                                        {
                                            returnValue = new RegexSettings(handlerSettings.Type)
                                            {
                                                ContentLineNumber = handlerSettings.ContentLineNumber,
                                                FieldConversion = handlerSettings.FieldConversion
                                            };
                                        }
                                        else
                                        {
                                            returnValue = new RegexSettings();
                                        }
                                        foreach (var pair in fieldExpressions)
                                        {
                                            returnValue.FieldExpressions[pair.Key] = pair.Value;
                                        }
                                        if (properties == null)
                                        {
                                            return returnValue;
                                        }
                                        returnValue.ContentLineNumber = (int) properties["ContentLineNumber"];
                                        returnValue.HeaderLineNumber = (int) properties["HeaderLineNumber"];
                                        return returnValue;
                                    }
                                },
                                {
                                    "indicies", (reader, serializer, handlerSettings, properties) =>
                                    {
                                        FixedIndexSettings returnValue;
                                        var fieldExpressions =
                                            serializer.Deserialize<IDictionary<string, int>>(reader);
                                        if (handlerSettings != null)
                                        {
                                            returnValue = new FixedIndexSettings(handlerSettings.Type)
                                            {
                                                FieldConversion = handlerSettings.FieldConversion
                                            };
                                        }
                                        else
                                        {
                                            returnValue = new FixedIndexSettings();
                                        }
                                        foreach (var pair in fieldExpressions)
                                        {
                                            returnValue.FieldIndices[pair.Value] = pair.Key;
                                        }
                                        if (properties == null)
                                        {
                                            return returnValue;
                                        }
                                        returnValue.ContentLineNumber = (int) properties["ContentLineNumber"];
                                        return returnValue;
                                    }
                                }
                            };

        private readonly IDictionary<string, Type> typeMap;

        public ContentHandlerFactory(ITypeMapFactory typeMapFactory)
        {
            typeMap = typeMapFactory.CreateTypeMap();
        }
        #region Implementation of IFileHandlerFactory

        public IContentHandler Create(string jsonData)
        {
            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(jsonData))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                ContentHandlerSettings settings = null;
                Dictionary<string, object> properties = null;
                while (jsonReader.Read())
                {
                    var key = jsonReader.Value as string;
                    if ((jsonReader.TokenType != JsonToken.PropertyName) || (string.IsNullOrEmpty(key)))
                    {
                        continue;
                    }
                    jsonReader.Read();
                    if (Factories.ContainsKey(key))
                    {
                        settings = Factories[key](jsonReader, serializer, settings, properties);
                        continue;
                    }
                    if (typeMap.ContainsKey(key))
                    {
                        var type = typeMap[key];
                        if (settings == null)
                        {
                            settings = new ContentHandlerSettings(type);
                        }
                        settings.FieldConversion = FieldConversionFactory.CreateDefault(type);
                        settings.Type = type;
                        var conversion = serializer.Deserialize<IDictionary<string, string>>(jsonReader);
                        foreach (var pair in conversion)
                        {
                            var converter = ConverterManager.GetConverter(pair.Value);
                            if (converter == null)
                            {
                                continue;
                            }
                            settings.FieldConversion[pair.Key] = converter.Function;
                        }
                    }
                    if (!string.Equals(key, "settings", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;                        
                    }
                    if (settings == null)
                    {
                        properties = serializer.Deserialize<Dictionary<string, object>>(jsonReader);
                    }
                    else
                    {
                        serializer.Populate(jsonReader, settings);
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
