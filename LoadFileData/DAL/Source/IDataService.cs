using System;
using System.Linq;
using LoadFileData.DAL.Entry;

namespace LoadFileData.DAL.Source
{
    public interface IDataService : IDisposable
    {
        IQueryable<DataSource> DataSource();
        IQueryable<DataEntry> DataEntries();
        void InsertDataSource(DataSource source);
        DataSource UpdateDataSource(DataSource source);
        void AddSourceError(SourceError error);
        void DeleteEntriesForSource(Guid sourceId);
        void DeleteErrorsForEntry(Guid entryId);
        void DeleteDataSource(Guid sourceId);
    }
}
