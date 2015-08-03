using System.Collections.Generic;
using System.Threading;
using LoadFileData.FileHandlers;

namespace LoadFileData.Monitors
{
    public class RecoverWorkflowsMonitor : IMonitor
    {
        private readonly IEnumerable<IRecoverWorkflow> workflows;

        public RecoverWorkflowsMonitor(IEnumerable<IRecoverWorkflow> workflows)
        {
            this.workflows = workflows;
        }

        public void StartMonitoring(CancellationToken token)
        {
            foreach (var handler in workflows)
            {
                handler.RecoverExistingFiles(token);
            }
        }

        public void StopMonitoring()
        {
        }
    }
}
