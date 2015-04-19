using LoadFileData.DAL.Entry;

namespace LoadFileData.WCF.Entry
{
    public class DataEntryErrorDataContract
    {
        public DataErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
