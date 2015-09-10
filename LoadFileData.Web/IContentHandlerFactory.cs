using LoadFileData.ContentHandlers;

namespace LoadFileData.Web
{
    public interface IContentHandlerFactory
    {
        IContentHandler Create(string jsonData);
    }
}
