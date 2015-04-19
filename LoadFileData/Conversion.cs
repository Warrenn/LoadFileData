using System;
using System.Collections.Generic;

namespace LoadFileData
{
    public class Conversion
    {

        public static Func<object, object> CreateFunction(string pattern)
        {
            //Func<string[],object, object>
            return o => o;
        }

        public static T ToConcrete<T>(IDictionary<string, object> dictionary) where T : new()
        {
            var instance = new T();
            var targetProperties = instance.GetType().GetProperties();

            foreach (var property in targetProperties)
            {
                object propVal;
                if (dictionary.TryGetValue(property.Name, out propVal))
                {
                    property.SetValue(instance, propVal, null);
                }
            }

            return instance;
        }

        public static IDictionary<string, object> ToDictionary(object staticObject)
        {
            var dictionary = new SortedDictionary<string, object>();
            var properties = staticObject.GetType().GetProperties();

            foreach (var property in properties)
            {
                dictionary[property.Name] = property.GetValue(staticObject, null);
            }

            return dictionary;
        }
    }
}
