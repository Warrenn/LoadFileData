using System;
using System.IO;
using System.Threading.Tasks;

namespace LoadFileData.WCF
{
    public interface IStreamManager
    {
        string StageStream(Guid sourceId, Stream stream);
        void RemoveStream(Guid sourceId);
        Task<MemoryStream> RetrieveData(Guid sourceId);
    }
}
