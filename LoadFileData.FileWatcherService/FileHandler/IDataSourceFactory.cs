using LoadFileData.DAL.Source;

namespace LoadFileData.FileWatcherService.FileHandler
{
    public interface IDataSourceFactory
    {
        DataSource CreateDataSource(string pathName);
    }
}
