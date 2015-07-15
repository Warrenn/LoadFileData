using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using LoadFileData.Constants;
using LoadFileData.ContentHandler;
using LoadFileData.ContentReader;
using LoadFileData.DAL;
using LoadFileData.DAL.Models;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace LoadFileData.FileHandler
{
    public class FileHandler<T> : IFileHandler, IRecoverWorkflow where T : new()
    {
        private readonly FileHandlerSettings<T> settings;
        private readonly IDataService service;
        private readonly IContentReader reader;
        private readonly IContentHandler<T> contentHandler;
        private readonly string destinationPath;

        public FileHandler(FileHandlerSettings<T> settings)
        {
            this.settings = settings;
            contentHandler = settings.ContentHandler;
            service = settings.Service;
            reader = settings.Reader;
            destinationPath = settings.DestinationPath;
        }

        public void Dispose()
        {
            service.Dispose();
        }

        public static string GetHash(Stream stream)
        {
            if ((stream == null) || (stream.Length <= 0)) return null;
            var algorithm = SHA1.Create();
            var hashBytes = algorithm.ComputeHash(stream);
            var hash = Encoding.Default.GetString(hashBytes);
            return hash;
        }

        public void ProcessFile(string fullPath, Stream stream, CancellationToken token)
        {
            var newGuid = Guid.NewGuid();
            var fileType = Path.GetExtension(fullPath);
            var destination = string.Format(destinationPath, newGuid, fileType);
            File.Copy(fullPath, destinationPath);
            stream.Seek(0, SeekOrigin.Begin);
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
                service.AddFileSource(fileSource);
                if (service.IsDuplicate(fileSource))
                {
                    return;
                }
                stream.Seek(0, SeekOrigin.Begin);
                var totalRows = reader.RowCount(stream);
                service.UpdateTotalRows(newGuid, totalRows);
                stream.Seek(0, SeekOrigin.Begin);
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
                fileSource.Status = FileStatus.Error;
                service.LogError(fileSource, ex);
                ExceptionPolicy.HandleException(ex, PolicyName.Default);
            }
        }

        public void ProcessFile(FileSource fileSource, ContentHandlerContext context, CancellationToken token)
        {
            var rowCount = 1;
            if (token.IsCancellationRequested)
            {
                return;
            }
            foreach (var dataEntry in contentHandler.HandleContent(context))
            {
                if (token.IsCancellationRequested)
                {
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
                var stream = File.OpenRead(fileSource.CurrentFileName);
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
