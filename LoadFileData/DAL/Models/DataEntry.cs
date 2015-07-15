using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.DAL.Models
{
    public class DataEntry
    {
        [Key]
        public Guid Id { get; set; }

        public string EntryType
        {
            get { return NonProxyType().Name; }
            set { }
        }

        public Type NonProxyType()
        {
            var entityType = GetType();
            if ((entityType.BaseType != null) && (entityType.Namespace == "System.Data.Entity.DynamicProxies"))
            {
                return entityType.BaseType;
            }
            return entityType;
        }

        public int Hash
        {
            get
            {
                Func<object, int> nullHash = obj => (obj == null) ? 0 : obj.GetHashCode();
                var returnValue = 17;
                returnValue = returnValue * 23 + nullHash(EntryType);
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
                        .Aggregate(returnValue, (current, property) => current * 23 + propertyHash(property));
                    return returnValue;
                }
            }
            set { }
        }


    }
}
