using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.DAL;

namespace LoadFileData.Web
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IDictionary<string, Type> typeMap;
        private readonly Lazy<Type> lazyClass;

        public ServiceFactory(ITypeMapFactory typeMapFactory)
        {
            lazyClass =  new Lazy<Type>(CreateServiceType);
            typeMap = typeMapFactory.CreateTypeMap();
        }

        private Type CreateServiceType()
        {
            var classBuilder = ClassBuilder.Build<DbContextBase>("WebDbContext");
            classBuilder = typeMap.Values.Aggregate(classBuilder, (current, type) => current.CreateSet(type));
            return classBuilder.ToType();
        }

        public IDataService Create()
        {
            var dbInstance = Activator.CreateInstance(lazyClass.Value);
            var dataServie = new DataService(dbInstance as DbContextBase);
            return dataServie;
        }
    }
}
