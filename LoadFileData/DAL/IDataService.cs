using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadFileData.DAL.Models;

namespace LoadFileData.DAL
{
    public interface IDataService : IDisposable
    {
        void AddFileSource(FileSource fileSource);
        void LogError(FileSource fileSource, Exception exception);
        void LogError(string filePath, Exception exception);
        void UpdateTotalRows(Guid fileId, int totalRows);

        void AddDataEntry(FileSource fileSource, object dataEntry, int rowCount);
        void MarkFileComplete(FileSource fileSource);

        IEnumerable<FileSource> PendingExtration(string settingsName);

        bool IsDuplicate(FileSource fileSource);
    }
}
