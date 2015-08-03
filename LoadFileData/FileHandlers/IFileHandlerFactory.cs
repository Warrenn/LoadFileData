using System.Collections.Generic;

namespace LoadFileData.FileHandlers
{
    public interface IFileHandlerFactory
    {
        IDictionary<string, IFileHandler> CreateFileHandlers();
    }
}
