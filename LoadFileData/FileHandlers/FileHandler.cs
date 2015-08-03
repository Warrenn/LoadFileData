using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using LoadFileData.ContentHandlers;
using LoadFileData.ContentReaders;
using LoadFileData.DAL;
using LoadFileData.DAL.Models;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace LoadFileData.FileHandlers
{
    public class FileHandler : IFileHandler, IRecoverWorkflow
    {
        private readonly FileHandlerSettings settings;
        private readonly IDataService service;
        private readonly IContentReader reader;
        private readonly IContentHandler contentHandler;
        private readonly string destinationPathTemplate;
        private readonly IStreamManager streamManager;

        public FileHandler(FileHandlerSettings settings)
        {
            this.settings = settings;
            contentHandler = settings.ContentHandler;
            service = settings.Service;
            reader = settings.Reader;
            destinationPathTemplate = settings.DestinationPathTemplate;
            streamManager = settings.StreamManager;
        }

        public void Dispose()
        {
            service.Dispose();
        }

        public static string GetHash(Stream stream)
        {
            if ((stream == null) || (stream.Length <= 0))
            {
                return null;
            }
            stream.Seek(0, SeekOrigin.Begin);
            var algorithm = SHA1.Create();
            var hashBytes = algorithm.ComputeHash(stream);
            var hash = Encoding.Default.GetString(hashBytes);
            return hash;
        }

        public void ProcessFile(string fullPath, Stream stream, CancellationToken token)
        {
            var newGuid = Guid.NewGuid();
            var fileType = Path.GetExtension(fullPath);
            var destination = string.Format(destinationPathTemplate, newGuid, fileType);
            streamManager.CopyFile(fullPath, destination);
            var hash = GetHash(stream);
            var fileSource = new FileSource
            {
                Id = newGuid,
                CurrentFileName = destination,
                CurrentRow = 0,
                DateEdit = DateTime.Now,
                FileHash = hash,
                OriginalFileName = fullPath,
                Status = FileStatus.PendingExtraction,
                SettingsName = settings.Name,
                TotalRows = 0
            };
            try
            {
                fileSource = service.AddFileSource(fileSource);
                if (service.IsDuplicate(fileSource))
                {
                    return;
                }
                var totalRows = reader.RowCount(stream);
                service.UpdateTotalRows(newGuid, totalRows);
                var enumerator = reader.ReadContent(stream);
                var context = new ContentHandlerContext
                {
                    Content = enumerator,
                    FileName = fullPath
                };
                ProcessFile(fileSource, context, token);
            }
            catch (Exception ex)
            {
                service.LogError(fileSource, ex);
                ExceptionPolicy.HandleException(ex, GetType().Name);
            }
        }

        public void ProcessFile(FileSource fileSource, ContentHandlerContext context, CancellationToken token)
        {
            var rowCount = 1;
            service.MarkFileExtracting(fileSource);
            if (token.IsCancellationRequested)
            {
                return;
            }
            foreach (var dataEntry in contentHandler.HandleContent(context))
            {
                if (token.IsCancellationRequested)
                {
                    service.MarkFilePaused(fileSource);
                    return;
                }
                service.AddDataEntry(fileSource, dataEntry, rowCount);
                rowCount++;
            }
            service.MarkFileComplete(fileSource);
        }

        public void ReportError(string fullPath, Exception exception)
        {
            service.LogError(fullPath, exception);
        }

        public void RecoverExistingFiles(CancellationToken token)
        {
            foreach (var fileSource in service.PendingExtration(settings.Name))
            {
                var stream = streamManager.OpenRead(fileSource.CurrentFileName);
                var enumerator = reader.ReadContent(stream);
                var context = new ContentHandlerContext
                {
                    Content = enumerator,
                    FileName = fileSource.OriginalFileName
                };
                ProcessFile(fileSource, context, token);
            }
        }
    }
}
