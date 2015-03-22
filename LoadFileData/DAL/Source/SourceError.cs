using System;

namespace LoadFileData.DAL.Source
{
    public class SourceError
    {
        public virtual int Id { get; set; }
        public virtual Guid DataSourceId { get; set; }
        public virtual SourceErrorType ErrorType { get; set; }
        public virtual string ErrorMessage { get; set; }
    }
}
