using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Conventions
{
    public class PropertyPath : IEnumerable<PropertyInfo>
    {
        private readonly List<PropertyInfo> components = new List<PropertyInfo>();

        public PropertyPath(IEnumerable<PropertyInfo> components)
        {
            if (components == null) return;
            this.components.AddRange(components);
        }

        public PropertyPath(PropertyInfo component)
        {
            if (component == null) return;
            components.Add(component);
        }

        private PropertyPath()
        {
            
        }

        public int Count => components.Count;

        public PropertyInfo this[int index] => components[index];

        public void Add(PropertyInfo info)
        {
            components.Add(info);
        }

        public static PropertyPath Empty => new PropertyPath();

        public override string ToString()
        {
            var propertyPathName = new StringBuilder();

            foreach (var info in components)
            {
                propertyPathName.Append(info.Name);
                propertyPathName.Append('_');
            }

            return propertyPathName.ToString(0, propertyPathName.Length - 1);
        }

        #region Equality Members

        public bool Equals(PropertyPath other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) ||
                   components.SequenceEqual(other.components, new DynamicEqualityComparer<PropertyInfo>(IsSameAs));
        }

        private static bool IsSameAs(PropertyInfo propertyInfo, PropertyInfo otherPropertyInfo) =>
            (propertyInfo == otherPropertyInfo) ||
            (propertyInfo.Name == otherPropertyInfo.Name
             && (propertyInfo.DeclaringType == otherPropertyInfo.DeclaringType
                 || propertyInfo.DeclaringType.IsSubclassOf(otherPropertyInfo.DeclaringType)
                 || otherPropertyInfo.DeclaringType.IsSubclassOf(propertyInfo.DeclaringType)
                 || propertyInfo.DeclaringType.GetInterfaces().Contains(otherPropertyInfo.DeclaringType)
                 || otherPropertyInfo.DeclaringType.GetInterfaces().Contains(propertyInfo.DeclaringType)));

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof(PropertyPath) && Equals((PropertyPath)obj);
        }

        public override int GetHashCode() => components.Aggregate(0, (t, n) => t + n.GetHashCode());

        public static bool operator ==(PropertyPath left, PropertyPath right) => Equals(left, right);

        public static bool operator !=(PropertyPath left, PropertyPath right) => !Equals(left, right);

        #endregion

        #region IEnumerable Members

        IEnumerator<PropertyInfo> IEnumerable<PropertyInfo>.GetEnumerator() => components.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => components.GetEnumerator();

        #endregion
    }
}
