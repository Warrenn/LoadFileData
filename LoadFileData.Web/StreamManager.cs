using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IDictionary<string, string> DataEntries()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> Definitions()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
