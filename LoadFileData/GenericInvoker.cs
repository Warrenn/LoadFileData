using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace LoadFileData
{
    public class GenericInvoker : DynamicObject
    {
        private readonly object instance;
        private readonly Type staticType;

        private static readonly ConcurrentDictionary<string, Lazy<MethodInfo>> MethodInfos =
            new ConcurrentDictionary<string, Lazy<MethodInfo>>();

        public const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase;

        public const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public GenericInvoker(Type staticType)
        {
            this.staticType = staticType;
        }

        public GenericInvoker(object instance)
        {
            this.instance = instance;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (args.Length < 1)
            {
                return false;
            }
            if (args[0] == null)
            {
                return false;
            }

            Type[] genericTypes = null;
            var type = args[0] as Type;
            if (type != null)
            {
                genericTypes = new[] { type };
            }
            var types = args[0] as Type[];
            if (types != null)
            {
                genericTypes = types;
            }
            if ((genericTypes == null) || (genericTypes.Length == 0))
            {
                return false;
            }
            var callingArgs = new object[args.Length - 1];
            for (var i = 1; i < args.Length; i++)
            {
                callingArgs[i - 1] = args[i];
            }
            if (staticType != null)
            {
                result = InvokeStatic(
                    binder.Name,
                    staticType,
                    genericTypes,
                    callingArgs);
                return true;
            }
            if (instance == null)
            {
                return false;
            }

            result = InvokeInstance(
                binder.Name,
                instance,
                genericTypes,
                callingArgs);
            return true;
        }

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
