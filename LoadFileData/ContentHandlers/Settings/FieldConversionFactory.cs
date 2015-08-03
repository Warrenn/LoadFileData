using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public static class FieldConversionFactory
    {
        public static IDictionary<string, Func<object, object>> CreateDefault(Type type)
        {
            var returnValue = new Dictionary<string, Func<object, object>>();

            foreach (var field in type.GetFields())
            {
                var fieldInfo = field;
                var fieldType = fieldInfo.FieldType;

                returnValue[field.Name] = o => TryParser.ChangeType(o, fieldType);
            }
            return returnValue;
        }

    }
}