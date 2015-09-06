using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.DAL;

namespace LoadFileData.Web
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly ITypeMapFactory typeMapFactory;

        public ServiceFactory(ITypeMapFactory typeMapFactory)
        {
            this.typeMapFactory = typeMapFactory;
        }

        public IDataService Create()
        {
            var typeMap = typeMapFactory.CreateTypeMap();
            var classBuilder = ClassBuilder.Build<DbContextBase>("WebDbContext");
            classBuilder = typeMap.Values.Aggregate(classBuilder, (current, type) => current.CreateSet(type));
            var dataServiceType = classBuilder.ToType();
            var dbInstance = Activator.CreateInstance(dataServiceType);
            var dataServie = new DataService(dbInstance as DbContextBase);
            return dataServie;
        }
    }
}
