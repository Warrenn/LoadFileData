namespace LoadFileData.DAL.Models
{
    public enum FileStatus
    {
        PendingExtraction = 1,
        Extracting = 2,
        Completed = 4,
        Paused = 8,
        Error = 16
    }
}
