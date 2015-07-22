using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.FileHandlers
{
    public interface IFileHandlerFactory
    {
        IDictionary<string, IFileHandler> CreateFileHandlers();
    }
}
