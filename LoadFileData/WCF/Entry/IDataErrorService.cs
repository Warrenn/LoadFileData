using System.ServiceModel;
using LoadFileData.DAL.Entry;

namespace LoadFileData.WCF.Entry
{
    [ServiceContract]
    public partial interface IDataErrorService
    {
        [OperationContract]
        DataEntryUpsertResultDataContract UpsertDataEntry(DataEntry dataEntry);
    }
}
