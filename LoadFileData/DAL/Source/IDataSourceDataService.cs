using System;
using System.Linq;

namespace LoadFileData.DAL.Source
{
    public interface IDataSourceDataService : IDisposable
    {
        IQueryable<DataSource> DataSource();
        DataSource UpsertDataSource(DataSource source);
    }
}
