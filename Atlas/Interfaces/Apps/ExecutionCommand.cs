using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces.Apps
{
    internal struct ExecutionCommand
    {
        public string command;
        public string arguments;
    }
    internal enum ExecutionCommandStage
    {
        PreBuild, Build, PostBuild, Run
    }
}
