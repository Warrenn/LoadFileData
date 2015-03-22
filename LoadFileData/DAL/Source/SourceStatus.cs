namespace LoadFileData.DAL.Source
{
    public enum SourceStatus
    {
        PendingExtraction = 1,
        Extracting = 2,
        PendingTransformation = 3,
        Transforming = 4,
        Transformed = 5
    }
}
