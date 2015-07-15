using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandlers.Settings
{
    public static class FieldConversionFactory
    {
        public static IDictionary<string, Action<T, object>> CreateDefault<T>()
        {
            var type = typeof (T);
            var returnValue = new Dictionary<string, Action<T, object>>();
            dynamic genericInvoker = new GenericInvoker(typeof (TryParser));

            foreach (var field in type.GetFields())
            {
                var fieldInfo = field;
                var fieldType = fieldInfo.FieldType;
                Action<T, object> convertAction;
                if (fieldType.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof (Nullable<>)))
                {
                    convertAction = (instance, fieldValue) =>
                    {
                        var defaultValue = genericInvoker.Nullable(fieldInfo.FieldType, fieldValue);
                        fieldInfo.SetValue(instance, defaultValue);
                    };
                }
                else
                {
                    convertAction = (instance, fieldValue) =>
                    {
                        var defaultValue = genericInvoker.Default(fieldInfo.FieldType, fieldValue);
                        fieldInfo.SetValue(instance, defaultValue);
                    };
                }
                returnValue[field.Name] = convertAction;
            }
            return returnValue;
        }

    }
}