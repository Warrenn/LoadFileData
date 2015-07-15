using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.DAL.Models;

namespace LoadFileData.DAL
{
    public class DataService : IDataService
    {
        private readonly DbContextBase context;

        public DataService(DbContextBase context)
        {
            this.context = context;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion

        #region Implementation of IDataService

        public void CommitChanges()
        {
            context.SaveChanges();
        }

        public FileSource AddFileSource(FileSource fileSource)
        {
            return context.FileSources.Add(fileSource);
        }

        public void LogError(FileSource fileSource, Exception exception)
        {
            var error = new Error
            {
                Date = DateTime.Now,
                ErrorMessage = exception.ToString(),
                FileId = fileSource.Id,
                FileName = fileSource.OriginalFileName
            };
            context.Errors.Add(error);
            ExceptionHandler.Try(() =>
            {
                context.SaveChanges();
            });
        }

        public void LogError(string filePath, Exception exception)
        {
            var error = new Error
            {
                Date = DateTime.Now,
                ErrorMessage = exception.ToString(),
                FileName = filePath
            };
            context.Errors.Add(error);
            ExceptionHandler.Try(() =>
            {
                context.SaveChanges();
            });
        }

        public void UpdateTotalRows(Guid fileId, int totalRows)
        {
            var entry = context.FileSources.Find(fileId);
            entry.TotalRows = totalRows;
            context.FileSources.AddOrUpdate(entry);
        }

        public object AddDataEntry(FileSource fileSource, object dataEntry, int rowCount)
        {
            var entry = dataEntry as DataEntry;
            if (entry == null)
            {
                return dataEntry;
            }
            entry.FileSource = fileSource;
            entry.FileSourceId = fileSource.Id;
            return context.Set(dataEntry.GetType()).Add(dataEntry);
        }

        public void MarkFileComplete(FileSource fileSource)
        {
            fileSource.Status = FileStatus.Completed;
            context.FileSources.AddOrUpdate(fileSource);
        }

        public IQueryable<FileSource> PendingExtration(string settingsName)
        {
            return context
                .FileSources
                .Where(source =>
                    source.Status == FileStatus.PendingExtraction &&
                    source.SettingsName == settingsName);
        }

        public bool IsDuplicate(FileSource fileSource)
        {
            return context
                .FileSources
                .Any(source =>
                    source.Id != fileSource.Id &&
                    source.FileHash == fileSource.FileHash &&
                    source.SettingsName == fileSource.SettingsName);
        }

        #endregion
    }
}
