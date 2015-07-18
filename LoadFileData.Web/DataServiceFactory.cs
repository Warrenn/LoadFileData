using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using LoadFileData.DAL;
using LoadFileData.DAL.Models;
using LoadFileData.Web.Constants;

namespace LoadFileData.Web
{
    public static class DataServiceFactory
    {
        public static object CreateDataContext(IDictionary<string, Type> jsonTypes)
        {
            var contextBuilder = ClassBuilder.Build("WebDbContext", typeof(DbContextBase));

            foreach (var jsonType in jsonTypes)
            {
                contextBuilder.CreateSet(jsonType.Value);
            }

            return AssemblyHelper
                .LoadableTypesOf<DataEntry>()
                .Aggregate(contextBuilder, (current, entryType) => current.CreateSet(entryType))
                .ToInstance();
        }
    }
}