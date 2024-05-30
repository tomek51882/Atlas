using Atlas.Core.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces.Apps
{
    internal interface IAppRunner
    {
        RunnerStatus RunnerStatus { get; }
        event Action<AppRunner> OnRunnerStopped;
        event Action<RunnerStatus> OnRunnerStatusChange;
    }
}
