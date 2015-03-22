using System;

namespace LoadFileData.DAL.Entry
{
    public class DataError
    {
        public virtual int Id { get; set; }
        public virtual DataEntry DataEntry { get; set; }
        public virtual Guid DataEntryId { get; set; }
        public virtual string Column { get; set; }
        public virtual DataErrorType ErrorType { get; set; }
        public virtual string Message { get; set; }
    }
}
