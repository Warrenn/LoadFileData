using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public static class FieldConversionFactory
    {
        public static IDictionary<string, Func<object, object>> CreateDefault<T>()
        {
            var type = typeof(T);
            var returnValue = new Dictionary<string, Func<object, object>>();
            dynamic genericInvoker = new GenericInvoker(typeof(TryParser));

            foreach (var field in type.GetFields())
            {
                var fieldInfo = field;
                var fieldType = fieldInfo.FieldType;
                var convertAction =
                    (fieldType.IsGenericType &&
                     (fieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        ? (Func<object, object>)(fieldValue => genericInvoker.Nullable(fieldInfo.FieldType, fieldValue))
                        : (fieldValue => genericInvoker.Default(fieldInfo.FieldType, fieldValue));
                returnValue[field.Name] = convertAction;
            }
            return returnValue;
        }

    }
}