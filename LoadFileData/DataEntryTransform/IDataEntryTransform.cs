using System.Collections.Generic;
using LoadFileData.DAL.Entry;

namespace LoadFileData.DataEntryTransform
{
    public interface IDataEntryTransform
    {
        IEnumerable<DataEntry> TransfromEntries(DataEntry entry);
        IEnumerable<DataError> ValidateEntry(DataEntry entry);
    }
}
