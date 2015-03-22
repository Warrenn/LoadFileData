namespace LoadFileData.DAL.Source
{
    public enum SourceErrorType
    {
        GenericError = 0,
        DuplicateFile = 1,
        FileTypeNotFound = 2,
        UnReadableFile = 3,
        ExceptionOccured = 5
    }
}
