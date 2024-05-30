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
        public IAppExecutable App { get; }
        public IAppRunner AppRunner { get; }

        internal AppRunnerHandler(IAppExecutable app, IAppRunner appRunner)
        {
            App = app;
            AppRunner = appRunner;
        }
    }
}
