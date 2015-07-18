using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LoadFileData
{
    public static class AssemblyHelper
    {
        private static readonly Lazy<Type[]> LazyAssemblyTypes = new Lazy<Type[]>(LoadAllAssemblyTypes);

        private static Type[] LoadAllAssemblyTypes()
        {
            return (
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in GetLoadableTypes(assembly)
                select type).ToArray();
        }

        public static Type[] AllTypesInDomain()
        {
            return LazyAssemblyTypes.Value;
        }

        public static IEnumerable<Type> LoadableTypesOf(Type baseType)
        {
            var allTypes = AllTypesInDomain();
            for (var i = 0; i < allTypes.Length; i++)
            {
                if (TypeIsLoadableFrom(allTypes[i], baseType))
                {
                    yield return allTypes[i];
                }
            }
        }

        public static bool TypeIsLoadableFrom(Type inheritedType, Type baseType)
        {
            return (inheritedType != baseType) && (baseType.IsAssignableFrom(inheritedType) || IsAssignableToGenericType(baseType, inheritedType));
        }

        public static IEnumerable<Type> LoadableTypesOf<T>()
        {
            var type = typeof (T);
            return LoadableTypesOf(type);
        }

        public static IEnumerable<Type> LoadableTypesOf<T>(IEnumerable<Type> types)
        {
            var type = typeof (T);
            return LoadableTypesOf(type, types);
        }

        public static IEnumerable<Type> LoadableTypesOf(Type baseType, IEnumerable<Type> types)
        {
            return
                from type in types
                where TypeIsLoadableFrom(baseType, type)
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
