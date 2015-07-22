using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.FileHandlers
{
    public interface IStreamManager
    {
        void CopyFile(string source, string destination);
        Stream OpenRead(string fullPath);
        IDictionary<string, string> DataEntries();
        IDictionary<string, string> Definitions();
    }
}
