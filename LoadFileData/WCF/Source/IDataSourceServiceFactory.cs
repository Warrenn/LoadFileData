using System;

namespace LoadFileData.WCF.Source
{
    public interface IDataSourceServiceFactory : IDisposable
    {
        IDataSourceService Create();
    }
}
