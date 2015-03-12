namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IRegexFieldsManager
    {
        string[] MapHeaders(string[] headers);
        void AddField(string fieldName, string regexPattern);
    }
}
