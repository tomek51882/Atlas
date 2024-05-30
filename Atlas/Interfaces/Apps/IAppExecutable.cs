using Atlas.Types;

namespace Atlas.Interfaces.Apps
{
    internal interface IAppExecutable
    {
        public string Name { get; }
        public AppType AppType { get; }
        public string ExecutablePath { get; }
        //public IAppRunner CurrentRunner { get; }
        //public string ExecutablePath { get; }
        //public string ExecutableArguments { get; }

        ExecutionCommand? PreBuild();
        ExecutionCommand? Build();
        ExecutionCommand? PostBuild();
        ExecutionCommand? Run();
        void Stop();
    }
}
