using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces.Apps
{
    internal abstract class AppBase : IAppExecutable
    {
        public string Name { get; internal set; }
        public AppType AppType { get; internal set; }
        public IAppRunner CurrentRunner { get; internal set; }

        public virtual ExecutionCommand? PreBuild() { return null; }
        public virtual ExecutionCommand? Build() { return null; }
        public virtual ExecutionCommand? PostBuild() { return null; }
        public virtual ExecutionCommand? Run() { return null; }
        public virtual void Stop() { }
        

    }
}
