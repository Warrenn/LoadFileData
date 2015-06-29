using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.DAL.Entry
{
    public interface IDataEntryServiceFactory
    {
        IDataEntryService CreateService();
    }
}
