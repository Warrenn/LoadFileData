using System;
using System.Collections.Generic;
using System.Linq;
using LoadFileData.DAL;

namespace LoadFileData.Web
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IDictionary<string, Type> mapping;

        public ServiceFactory(IDictionary<string,Type> mapping)
        {
            this.mapping = mapping;
        }

        public IDataService Create()
        {
            var classBuilder = ClassBuilder
                .Build<DbContextBase>("WebDbContext");
            classBuilder = mapping.Values.Aggregate(classBuilder, (current, type) => current.CreateSet(type));
            var instance = classBuilder.ToInstance();
            return instance as IDataService;
        }
    }
}
