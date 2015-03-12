using System;
using System.Collections.Generic;
using System.IO;

namespace LoadFileData.ETLLayer.FileHandler
{
    public interface IFileHandler : IDisposable
    {
        void InitiateTenant(string tenant);

        void ProcessFile(string fullPath, Stream stream);

        IDictionary<string, string> ValidateFile(string fullPath, Stream fileStream);

        void ProcessComplete(string fullPath);
        
        void ReportError(string fullPath, Exception exception);
    }
}
