using System;
using LoadFileData.DAL.Source;

namespace LoadFileData.WCF.Source
{
    public interface IDataSourceService : IDisposable
    {
        DataSource CreateDataSource(DataSourceMessageContract dataSource);
        void ExtractData(Guid sourceId);
        void RevertDataSource(Guid sourceId);
        DataSource UpdateDataSource(DataSource source);
        void ReportSourceError(SourceError error);
    }
}