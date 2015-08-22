using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public static class PropertyConversionFactory
    {
        public static IDictionary<string, Func<object, object>> CreateDefault(Type type)
        {
            var returnValue = new Dictionary<string, Func<object, object>>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var field in type.GetProperties())
            {
                var fieldInfo = field;
                var fieldType = fieldInfo.PropertyType;

                returnValue[field.Name] = o => TryParser.ChangeType(o, fieldType);
            }
            return returnValue;
        }

    }
}