using System;
using System.Data.Entity;
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
            var builder = ClassBuilder.CreateBaseBuilder();
            var typeMap = typeMapFactory.CreateTypeMap(builder);
            var classBuilder = ClassBuilder.Build<DbContextBase>("WebDbContext");
            classBuilder = typeMap.Values.Aggregate(classBuilder, (current, type) => current.CreateSet(type));
            var dataServiceType = classBuilder.ToType(builder);
            var dbInstance = Activator.CreateInstance(dataServiceType);
            var migType = typeof(Migration<>).MakeGenericType(dataServiceType);

            var initType = typeof(MigrateDatabaseToLatestVersion<,>).MakeGenericType(dataServiceType, migType);
            var initInst = Activator.CreateInstance(initType);

            dynamic genericDatabase = new GenericInvoker(typeof(Database));
            genericDatabase.SetInitializer(dataServiceType, initInst);

            var myClass = (DbContextBase)dbInstance;
            myClass.Database.Initialize(true);

            var dataServie = new DataService(dbInstance as DbContextBase);
            return dataServie;
        }
    }
}
