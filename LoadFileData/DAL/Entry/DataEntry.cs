using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LoadFileData.DAL.Source;

namespace LoadFileData.DAL.Entry
{
    public class DataEntry
    {
        public Guid Id { get; set; }
        public DateTime EditDate { get; set; }
        public virtual DataSource Source { get; set; }
        public virtual Guid SourceId { get; set; }
        public int? RowNo { get; set; }

        public virtual string EntryType
        {
            get { return NonProxyType().Name; }
            set { }
        }

        private Type NonProxyType()
        {
            var entityType = GetType();
            if ((entityType.BaseType != null) &&
                (entityType.Namespace == "System.Data.Entity.DynamicProxies"))
            {
                return entityType.BaseType;
            }
            return entityType;
        }

        public virtual int Hash
        {
            get
            {
                Func<object, int> nullHash = obj => (obj == null) ? 0 : obj.GetHashCode();
                var returnValue = 17;
                if (typeof(DataEntry) == NonProxyType()) return returnValue;
                Func<PropertyInfo, int> propertyHash = info =>
                    {
                        var value = info.GetValue(this);
                        return nullHash(value);
                    };
                unchecked
                {
                    returnValue = NonProxyType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(property => property.CanRead && property.GetValue(this) != null)
                        .Aggregate(returnValue, (current, property) => current*23 + propertyHash(property));
                    return returnValue;
                }
            }
            set { }
        }
    }
}