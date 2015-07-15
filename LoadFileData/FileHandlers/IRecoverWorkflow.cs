using System.Threading;

namespace LoadFileData.FileHandlers
{
    public interface IRecoverWorkflow
    {
        void RecoverExistingFiles(CancellationToken token);
    }
}
