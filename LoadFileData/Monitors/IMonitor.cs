using System.Threading;

namespace LoadFileData.Monitors
{
    public interface IMonitor
    {
        void StartMonitoring(CancellationToken token);
        void StopMonitoring();
    }
}