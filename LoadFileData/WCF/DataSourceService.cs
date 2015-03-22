using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using LoadFileData.Constants;
using LoadFileData.DAL.Source;
using LoadFileData.WCF.Source;

namespace LoadFileData.WCF
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataSourceService : IDataSourceService
    {
        private readonly IDataSourceDataService dataService;

        public DataSourceService(IDataSourceDataService dataService)
        {
            this.dataService = dataService;
        }

        #region IDataSourceService Members

        public DataSource CreateDataSource(DataSourceMessageContract dataSource)
        {
            throw new NotImplementedException();
        }

        public void ExtractData(Guid sourceId)
        {
            throw new NotImplementedException();
        }

        public void RevertDataSource(Guid sourceId)
        {
            throw new NotImplementedException();
        }

        public DataSource UpdateDataSource(DataSource source)
        {
            throw new NotImplementedException();
        }

        public void ReportSourceError(SourceError error)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            dataService.Dispose(PolicyName.Disposable);
        }

        #endregion
    }
}
