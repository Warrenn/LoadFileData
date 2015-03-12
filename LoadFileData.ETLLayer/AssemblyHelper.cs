using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LoadFileData.ETLLayer
{
    public static class AssemblyHelper
    {

        public static IEnumerable<Type> LoadableTypesOf<T>()
        {
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies() 
                from type in LoadableTypesOf<T>(assembly)
                select type;
        }

        public static IEnumerable<Type> LoadableTypesOf<T>(Assembly assembly)
        {
            return
                from type in GetLoadableTypes(assembly)
                where (type.IsAssignableFrom(typeof (T)) ||
                       IsAssignableToGenericType(type, typeof (T)))
                select type;
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;


            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

    }
}
