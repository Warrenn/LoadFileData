using System;
using System.Data.Entity;

namespace LoadFileData
{
    public static class ClassBuilderExtension
    {
        public static ClassBuilder.FluentClassBuilder CreateSet(this ClassBuilder.FluentClassBuilder description,
            Type propertyType, string name)
        {
            var dbSetType = typeof (DbSet<>).MakeGenericType(propertyType);
            return description.Property(dbSetType, name);
        }

        public static ClassBuilder.FluentClassBuilder CreateSet(this ClassBuilder.FluentClassBuilder description,
            Type propertyType)
        {
            return CreateSet(description, propertyType, propertyType.Name);
        }

        public static ClassBuilder.FluentClassBuilder CreateSet<T>(this ClassBuilder.FluentClassBuilder description,
            string name)
        {
            return CreateSet(description, typeof(T), name);
        }

        public static ClassBuilder.FluentClassBuilder CreateSet<T>(this ClassBuilder.FluentClassBuilder description)
        {
            return CreateSet(description, typeof(T), typeof(T).Name);

        }
    }
}
