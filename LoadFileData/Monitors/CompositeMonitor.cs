using System.Collections.Generic;
using System.Threading;

namespace LoadFileData.Monitors
{
    public class CompositeMonitor : IMonitor
    {
        private readonly IEnumerable<IMonitor> monitors;

        public CompositeMonitor(IEnumerable<IMonitor> monitors)
        {
            this.monitors = monitors;
        }

        #region Implementation of IFolderMonitor

        public void StartMonitoring(CancellationToken token)
        {
            foreach (var folderMonitor in monitors)
            {
                folderMonitor.StartMonitoring(token);
            }
        }

        public void StopMonitoring()
        {
            foreach (var folderMonitor in monitors)
            {
                folderMonitor.StopMonitoring();
            }
        }

        #endregion
    }
}
