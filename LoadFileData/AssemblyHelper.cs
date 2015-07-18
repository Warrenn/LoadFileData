using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LoadFileData
{
    public static class AssemblyHelper
    {

        public static IEnumerable<Type> LoadableTypesOf(Type loadableType)
        {
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in LoadableTypesOf(loadableType, assembly)
                select type;
        }

        public static IEnumerable<Type> LoadableTypesOf<T>()
        {
            var type = typeof (T);
            return LoadableTypesOf(type);
        }

        public static IEnumerable<Type> LoadableTypesOf<T>(Assembly assembly)
        {
            var type = typeof (T);
            return LoadableTypesOf(type, assembly);
        }

        public static IEnumerable<Type> LoadableTypesOf(Type loadableType, Assembly assembly)
        {
            return
                from type in GetLoadableTypes(assembly)
                where (type.IsAssignableFrom(loadableType) || IsAssignableToGenericType(type, loadableType))
                select type;
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }


            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

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
