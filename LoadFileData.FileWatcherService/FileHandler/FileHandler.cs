using System;
using System.IO;
using LoadFileData.Constants;
using LoadFileData.DAL.Source;
using LoadFileData.WCF.Source;

namespace LoadFileData.FileWatcherService.FileHandler
{
    public class FileHandler : IFileHandler
    {
        private readonly IDataSourceServiceFactory dataSourceServiceFactory;
        private readonly IDataSourceFactory dataSourceFactory;
        private readonly IDataSourceService dataSource;

        protected FileHandler(IDataSourceServiceFactory dataSourceServiceFactory, IDataSourceFactory dataSourceFactory)
        {
            this.dataSourceServiceFactory = dataSourceServiceFactory;
            this.dataSourceFactory = dataSourceFactory;
            dataSource = dataSourceServiceFactory.Create();
        }


        public virtual Guid? GetSourceId(string fullPath)
        {
            var name = Path.GetFileNameWithoutExtension(fullPath) ?? string.Empty;
            return TryParser.Nullable<Guid>(name);
        }


        #region IFileHandler Members

        public virtual void ProcessFile(string fullPath, Stream stream)
        {
            var sourceId = GetSourceId(fullPath);
            if (sourceId != null)
            {
                dataSource.ExtractData((Guid)sourceId);
                return;
            }
            var source = dataSourceFactory.CreateDataSource(fullPath);
            var message = new DataSourceMessageContract
            {
                FileByteStream = stream,
                //Source = source
            };
            dataSource.CreateDataSource(message);
        }

        public virtual void ReportError(string fullPath, Exception exception)
        {
            var sourceId = GetSourceId(fullPath);
            if (sourceId == null)
            {
                return;
            }
            var error = new SourceError
            {
                DataSourceId = (Guid)sourceId,
                ErrorMessage = exception.ToString(),
                ErrorType = SourceErrorType.ExceptionOccured
            };
            dataSource.ReportSourceError(error);
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            dataSourceServiceFactory.Dispose(PolicyName.Disposable);
        }

        #endregion
    }
}
