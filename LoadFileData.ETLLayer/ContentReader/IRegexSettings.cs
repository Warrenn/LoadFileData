using System;

namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IRegexSettings
    {
        void SetFieldRegexMapping(string fieldName, string regexPattern, Func<object, object> conversion);
    }
}
