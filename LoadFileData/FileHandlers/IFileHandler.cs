﻿using System;
using System.IO;
using System.Threading;

namespace LoadFileData.FileHandlers
{
    public interface IFileHandler : IDisposable
    {
        void ProcessFile(string fullPath, Stream stream, CancellationToken token);
        
        void ReportError(string fullPath, Exception exception);
    }
}
