using System;
using System.IO;
using LoadFileData.FileHandlers;

namespace LoadFileData.Web
{
    public class StreamManager : IStreamManager
    {
        #region Implementation of IStreamManager

        public void CopyFile(string source, string destination)
        {
            throw new NotImplementedException();
        }

        public Stream OpenRead(string fullPath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
