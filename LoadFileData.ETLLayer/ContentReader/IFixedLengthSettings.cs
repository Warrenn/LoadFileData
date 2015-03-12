namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IFixedLengthSettings
    {
        void AddFieldWidth(string fieldName, int width);
        string[] CommentTokens { set; }
        bool TrimWhiteSpace { set; }
    }
}
