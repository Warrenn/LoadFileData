using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.DAL.Entry
{
    public interface IDataEntryService : IDisposable
    {
        void InsertDataEntry(DataEntry dataEntry);
        void UpdateDataEntry(DataEntry dataEntry);
        void AddEntryError(DataError error);
        void Commit();
    }
}
