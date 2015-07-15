using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData
{
    public static class ClassBuilderExtension
    {
        public static ClassBuilder.FluentPropertyDescription CreateSet(this ClassBuilder.FluentPropertyDescription description,
            Type propertyType, string name)
        {
            var dbSetType = typeof (DbSet<>).MakeGenericType(propertyType);
            return description.Property(dbSetType, name);
        }

        public static ClassBuilder.FluentPropertyDescription CreateSet(this ClassBuilder.FluentPropertyDescription description,
            Type propertyType)
        {
            return CreateSet(description, propertyType, propertyType.Name);
        }

        public static ClassBuilder.FluentPropertyDescription CreateSet<T>(this ClassBuilder.FluentPropertyDescription description,
            string name)
        {
            return CreateSet(description, typeof(T), name);
        }

        public static ClassBuilder.FluentPropertyDescription CreateSet<T>(this ClassBuilder.FluentPropertyDescription description)
        {
            return CreateSet(description, typeof(T), typeof(T).Name);

        }
    }
}
