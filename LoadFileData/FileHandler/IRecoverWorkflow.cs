using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LoadFileData.FileHandler
{
    public interface IRecoverWorkflow
    {
        void RecoverExistingFiles(CancellationToken token);
    }
}
