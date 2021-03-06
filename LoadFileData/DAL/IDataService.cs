﻿using System;
using System.Linq;
using LoadFileData.DAL.Models;

namespace LoadFileData.DAL
{
    public interface IDataService : IDisposable
    {
        FileSource AddFileSource(FileSource fileSource);
        void MarkFileExtracting(FileSource fileSource);
        void MarkFilePaused(FileSource fileSource);
        void LogError(string filePath, Exception exception);
        void UpdateTotalRows(FileSource fileSource, int totalRows);
        object AddDataEntry(FileSource fileSource, object dataEntry, int rowCount);
        void MarkFileComplete(FileSource fileSource);
        IQueryable<FileSource> PendingExtration(string settingsName);
        bool IsDuplicate(FileSource fileSource);
    }
}
