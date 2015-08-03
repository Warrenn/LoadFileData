using System.IO;

namespace LoadFileData.FileHandlers
{
    public interface IStreamManager
    {
        void CopyFile(string source, string destination);
        Stream OpenRead(string fullPath);
    }
}
