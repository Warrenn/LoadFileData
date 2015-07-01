using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace LoadFileData
{
    public class GenericHelper
    {
        private static readonly ConcurrentDictionary<string, Lazy<MethodInfo>> MethodInfos =
            new ConcurrentDictionary<string, Lazy<MethodInfo>>();

        public const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase;

        public const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        private static object Invoke(
            string methodName,
            object instance,
            Type callingType,
            Type[] genericTypes,
            object[] arguments)
        {
            var argumentTypes = arguments.Select(a => a.GetType()).ToArray();
            var flags = (instance == null) ? StaticFlags : InstanceFlags;
            var methodInfo = MethodInfo(methodName, callingType, flags, argumentTypes);
            if (methodInfo == null)
            {
                throw new MissingMethodException(callingType.FullName, methodName);
            }
            var generic = methodInfo.MakeGenericMethod(genericTypes);
            return generic.Invoke(instance, arguments);
        }

        public static MethodInfo MethodInfo(string methodName, Type callingType, BindingFlags flags,
            Type[] argumentTypes)
        {
            var key = string.Format("{0}.{1}", callingType.GUID, methodName);
            var lazy = MethodInfos
                .GetOrAdd(key, new Lazy<MethodInfo>(
                    () => callingType.GetMethod(methodName, flags, null, argumentTypes, null)));
            var methodInfo = lazy.Value;
            return methodInfo;
        }

        public static object InvokeStatic(string methodName, Type staticType, Type[] genericTypes,
            params object[] arguments)
        {
            return Invoke(methodName, null, staticType, genericTypes, arguments);
        }

        public static object InvokeInstance(string methodName, object instance, Type[] genericTypes,
            params object[] arguments)
        {
            return (instance == null) ? null : Invoke(methodName, instance, instance.GetType(), genericTypes, arguments);
        }
    }
}
