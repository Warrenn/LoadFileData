using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.DAL;

namespace LoadFileData.Web
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IDictionary<string, Type> typeMap;

        public ServiceFactory(ITypeMapFactory typeMapFactory)
        {
            typeMap = typeMapFactory.CreateTypeMap();
        }

        public IDataService Create()
        {
            var classBuilder = ClassBuilder
                .Build<DbContextBase>("WebDbContext");
            classBuilder = typeMap.Values.Aggregate(classBuilder, (current, type) => current.CreateSet(type));
            var dbInstance = classBuilder.ToInstance();
            var dataServie = new DataService(dbInstance as DbContextBase);
            return dataServie;
        }
    }
}
