namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IDelimiteredSettings : IRegexSettings
    {
        string[] Delimiters { set; }
        string[] CommentTokens { set; }
        bool TrimWhiteSpace { set; }
    }
}
