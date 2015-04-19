using System;
using System.ServiceModel;
using System.Threading.Tasks;
using LoadFileData.DAL.Source;

namespace LoadFileData.WCF.Source
{
    [ServiceContract]
    public interface IDataSourceService : IDisposable
    {
        [OperationContract(IsOneWay = true)]
        void CreateDataSource(DataSourceMessageContract dataSource);

        [OperationContract(IsOneWay = true)]
        void ExtractData(Guid sourceId);

        [OperationContract(IsOneWay = true)]
        void TransformDataSource(Guid sourceId);

        [OperationContract(IsOneWay = true)]
        void RevertDataSource(Guid sourceId);

        [OperationContract(IsOneWay = true)]
        void RevertTransformation(Guid sourceId);

        [OperationContract]
        Task<DataSource> UpdateDataSource(DataSource source);

        [OperationContract]
        Task<DataSource> GetDataSource(Guid sourceId);

        [OperationContract(IsOneWay = true)]
        void ReportSourceError(SourceError error);
    }
}