using System;
using System.Collections.Generic;

namespace LoadFileData.ContentHandler.Settings
{
    public static class FieldConversionFactory
    {
        private static IDictionary<string, Action<T, object>> Create<T>(Func<Type, object, object> tryParseFunc)
        {
            var type = typeof(T);
            var returnValue = new Dictionary<string, Action<T, object>>();

            foreach (var field in type.GetFields())
            {
                var fieldInfo = field;
                Action<T, object> convertAction = (instance, fieldValue) =>
                {
                    var defaultValue = tryParseFunc(fieldInfo.FieldType, fieldValue);
                    fieldInfo.SetValue(instance, defaultValue);
                };
                returnValue[field.Name] = convertAction;
            }
            return returnValue;
        }

        public static IDictionary<string, Action<T, object>> CreateDefault<T>()
        {
            Func<Type, object, object> func = (t, o) => GenericInvoker.InvokeStatic(
                "Default",
                typeof (TryParser),
                new[] {t}, o);
            return Create<T>(func);
        }

        public static IDictionary<string, Action<T, object>> CreateNullable<T>()
        {
            Func<Type, object, object> func = (t, o) => GenericInvoker.InvokeStatic(
                "Nullable",
                typeof(TryParser),
                new[] { t }, o);
            return Create<T>(func);
        }
    }
}