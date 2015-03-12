using System;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IRegexSettings
    {
        void SetFieldRegexMapping(string fieldName, string regexPattern, Action<object, object> conversion);
    }
}
