using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.Constants;
using LoadFileData.ContentHandler;
using LoadFileData.ContentReader;
using LoadFileData.DAL.Entry;
using LoadFileData.DAL.Source;
using LoadFileData.DataEntryTransform;
using LoadFileData.WCF.Source;

namespace LoadFileData.WCF
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataSourceService : IDataSourceService
    {
        private readonly IDataService dataService;
        private readonly IEnumerable<ContentProcess> contentHandlers;
        private readonly IStreamManager streamManager;
        private readonly IDataEntryTransformFactory dataEntryTransformFactory;
        private readonly IDataEntryServiceFactory dataEntryServiceFactory;

        public static string GetHash(Stream stream)
        {
            if ((stream == null) || (stream.Length <= 0)) return null;
            var algorithm = SHA1.Create();
            var hashBytes = algorithm.ComputeHash(stream);
            return Encoding.Default.GetString(hashBytes);
        }

        public DataSourceService(
            IDataService dataService,
            IEnumerable<ContentProcess> contentHandlers,
            IStreamManager streamManager,
            IDataEntryTransformFactory dataEntryTransformFactory,
            IDataEntryServiceFactory dataEntryServiceFactory)
        {
            this.dataService = dataService;
            this.contentHandlers = contentHandlers;
            this.streamManager = streamManager;
            this.dataEntryTransformFactory = dataEntryTransformFactory;
            this.dataEntryServiceFactory = dataEntryServiceFactory;
        }

        #region IDataSourceService Members

        public void CreateDataSource(DataSourceMessageContract dataSource)
        {
            var fileStream = dataSource.FileByteStream;
            var hash = GetHash(fileStream);
            var searchEntry = dataService.DataSource().FirstOrDefault(source => source.FileHash == hash);
            if (searchEntry != null)
            {
                var sourceError = new SourceError
                {
                    DataSourceId = searchEntry.Id,
                    ErrorType = SourceErrorType.DuplicateFile,
                    ErrorMessage = DataSourceServiceResx.DataSourceService_CreateDataSource_Duplicate_Source
                };
                ReportSourceError(sourceError);
                return;
            }
            var sourceId = Guid.NewGuid();
            fileStream.Position = 0;
            var newFileName = streamManager.StageStream(sourceId, fileStream);
            var newEntry = new DataSource
            {
                Id = sourceId,
                FileHash = hash,
                InputStatus = SourceStatus.PendingExtraction,
                DateEdit = DateTime.Now,
                HandlerName = dataSource.HandlerName,
                OriginalFileName = dataSource.OriginalFileName,
                CurrentFileName = newFileName,
                MediaType = dataSource.MediaType,
                UserName = dataSource.UserName
            };
            dataService.InsertDataSource(newEntry);
        }

        public async void ExtractData(Guid sourceId)
        {
            var source = dataService.DataSource().FirstOrDefault(d => d.Id == sourceId);
            if (source == null) return;
            if (source.InputStatus != SourceStatus.PendingExtraction) return;

            source.InputStatus = SourceStatus.Extracting;
            dataService.UpdateDataSource(source);

            var stream = await streamManager.RetrieveData(sourceId);
            var errorInSource = false;

            foreach (var contentHandler in contentHandlers)
            {
                var rowNumber = 1;
                foreach (var rowData in contentHandler.Reader.ReadContent(stream))
                {
                    foreach (var rowEntry in contentHandler.Handler.ProcessRowData(rowData).Where(e => e != null))
                    {
                        var transform = dataEntryTransformFactory.CreateTransform(rowEntry.GetType().GUID);

                        rowEntry.RowNo = rowNumber;
                        rowEntry.SourceId = sourceId;
                        rowEntry.UserName = source.UserName;

                        foreach (var dataError in transform.ValidateEntry(rowEntry))
                        {
                            errorInSource = true;
                            rowEntry.Errors.Add(dataError);
                        } 
                    }
                    rowNumber++;
                }
            }

            source.InputStatus = SourceStatus.PendingTransformation;
            dataService.UpdateDataSource(source);

            if (!errorInSource)
            {
                TransformDataSource(sourceId);
            }

        }

        public void TransformDataSource(Guid sourceId)
        {
            var source = dataService.DataSource().FirstOrDefault(d => d.Id == sourceId);
            if (source == null) return;
            if (source.InputStatus != SourceStatus.PendingTransformation) return;

            source.InputStatus = SourceStatus.Extracting;
            dataService.UpdateDataSource(source);

            var entries =
                from dataEntry in dataService
                    .DataEntries()
                    .Where(e => e.SourceId == sourceId)
                let transform = dataEntryTransformFactory.CreateTransform(dataEntry.GetType().GUID)
                from entry in transform.TransfromEntries(dataEntry)
                select entry;
            
            using (var service = dataEntryServiceFactory.CreateService())
            {
                foreach (var entry in entries)
                {
                    service.InsertDataEntry(entry);
                }
                service.Commit();
            }
        }

        public void RevertDataSource(Guid sourceId)
        {
            dataService.DeleteDataSource(sourceId);
        }

        public void RevertTransformation(Guid sourceId)
        {
            dataService.DeleteEntriesForSource(sourceId);
        }

        public async Task<DataSource> UpdateDataSource(DataSource source)
        {
            return await Task.FromResult(dataService.UpdateDataSource(source));
        }

        public async Task<DataSource> GetDataSource(Guid sourceId)
        {
            return await Task.FromResult(dataService.DataSource().FirstOrDefault(d=>d.Id == sourceId));
        }

        public void ReportSourceError(SourceError error)
        {
            dataService.AddSourceError(error);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            dataService.Dispose(PolicyName.Disposable);
        }

        #endregion
    }
}
