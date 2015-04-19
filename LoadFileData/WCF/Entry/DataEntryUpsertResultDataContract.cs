using System;
using System.Collections.Generic;

namespace LoadFileData.WCF.Entry
{
    public class DataEntryUpsertResultDataContract
    {
        public Guid EntryId { get; set; }
        public bool Succeeded { get; set; }
        public IEnumerable<DataEntryErrorDataContract> Errors { get; set; }
    }
}
