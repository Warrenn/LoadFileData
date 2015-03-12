namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IRegexSettings
    {
        string[] MapHeaders(string[] headers);
        void SetFieldRegexMapping(string fieldName, string regexPattern);
    }
}
