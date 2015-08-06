using System.Collections.Generic;

namespace LoadFileData.FileHandlers
{
    public interface IFileHandlerSettingsFactory
    {
        IEnumerable<FileHandlerSettings> CreateFileHandlers();
    }
}
