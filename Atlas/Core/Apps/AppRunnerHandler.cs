using Atlas.Interfaces.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Core.Apps
{
    internal struct AppRunnerHandler
    {
        public IAppRunner AppRunner { get; init; }

        internal AppRunnerHandler(IAppRunner appRunner)
        {
            AppRunner = appRunner;
        }
    }
}
