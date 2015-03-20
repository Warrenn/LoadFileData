using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ETLLayer.FileHandler
{
    public interface IFileHandler : IDisposable
    {
        void ProcessFile(string fullPath, Stream stream);

        void ProcessComplete(string fullPath);
        
        void ReportError(string fullPath, Exception exception);
    }
}
