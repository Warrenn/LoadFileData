using LoadFileData.ETLLayer.FileReader;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IDelimiteredFieldsManager : IRegexFieldsManager
    {
        void SetDelimiters(params string[] delimiters);
    }
}
