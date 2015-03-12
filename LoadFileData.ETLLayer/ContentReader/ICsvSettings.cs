namespace LoadFileData.ETLLayer.ContentReader
{
    public interface ICsvSettings
    {
        string[] CommentTokens { set; }
        bool TrimWhiteSpace { set; }
    }
}
