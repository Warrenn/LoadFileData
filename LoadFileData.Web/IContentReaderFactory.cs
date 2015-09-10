using LoadFileData.ContentReaders;

namespace LoadFileData.Web
{
    public interface IContentReaderFactory
    {
        IContentReader Create(string jsonData);
    }
}
