using System;
using System.Collections.Generic;

namespace LoadFileData.DAL.Source
{
    public class DataSource
    {
        public virtual Guid Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual DateTime DateEdit { get; set; }
        public virtual SourceStatus InputStatus { get; set; }
        public virtual ICollection<SourceError> SourceErrors { get; set; }
        public virtual string FileHash { get; set; }
        public virtual string OriginalFileName { get; set; }
        public virtual string CurrentFileName { get; set; }
        public virtual string HandlerName { get; set; }
        public virtual string MediaType { get; set; }
        //public virtual ICollection<DataEntry> DataEntries { get; set; }

        private Type NonProxyType()
        {
            var entityType = GetType();
            if ((entityType.BaseType != null) && (entityType.Namespace == "System.Data.Entity.DynamicProxies"))
            {
                return entityType.BaseType;
            }
            return entityType;
        }

        public virtual string SourceType
        {
            get { return NonProxyType().Name; }
            set { }
        }

    }
}
