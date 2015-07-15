using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace LoadFileData
{
    public class DynamicProperties : DynamicObject
    {
        private readonly object instance;
        private readonly Type instanceType;
        private readonly string instanceGuid;

        private static readonly ConcurrentDictionary<string, Lazy<PropertyInfo>> MethodInfos =
            new ConcurrentDictionary<string, Lazy<PropertyInfo>>();
        private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public DynamicProperties(Type instanceType)
            : this(Activator.CreateInstance(instanceType))
        {
        }

        public DynamicProperties(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            this.instance = instance;
            instanceType = instance.GetType();
            instanceGuid = instanceType.GUID + ".";
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            var key = instanceGuid + propertyName;
            var lazy = MethodInfos
                .GetOrAdd(key, new Lazy<PropertyInfo>(
                    () => instanceType.GetProperty(propertyName, InstanceFlags)));
            return lazy.Value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (binder.Name == "Type")
            {
                result = instanceType;
                return true;
            }
            var propertyInfo = GetPropertyInfo(binder.Name);
            if (propertyInfo == null)
            {
                return false;
            }
            result = propertyInfo.GetMethod.Invoke(instance, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyInfo = GetPropertyInfo(binder.Name);
            if (propertyInfo == null || binder.Name == "Type")
            {
                return false;
            }
            propertyInfo.SetMethod.Invoke(instance, new[] { value });
            return true;
        }
    }
}
