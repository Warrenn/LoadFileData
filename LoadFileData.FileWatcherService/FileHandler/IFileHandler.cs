using System;
using System.IO;

namespace LoadFileData.FileWatcherService.FileHandler
{
    public interface IFileHandler : IDisposable
    {
        void ProcessFile(string fullPath, Stream stream);
        
        void ReportError(string fullPath, Exception exception);
    }
}
