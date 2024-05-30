using Atlas.Core.Apps;
using Atlas.Interfaces;
using Atlas.Interfaces.Apps;
using Atlas.Models.DTOs;

namespace Atlas.Services
{
    internal class AppsService: IAppsService
    {
        public List<IAppExecutable> Apps { get; private set; } = new List<IAppExecutable>();
        public List<IAppRunner> Runners { get; private set; } = new List<IAppRunner>();

        public event Action OnAppListChange = delegate { };
        public event Action<IAppRunner, IAppExecutable> OnRunnerStarted = delegate { };
        
        public AppsService() 
        {
            
        }

        public void AddApp(IAppExecutable app)
        {
            //TODO: Check for duplicates
            Apps.Add(app);
            OnAppListChange.Invoke();
        }

        public AppRunnerHandler ExecuteApp(IAppExecutable app)
        {
            var runner = new AppRunner(app, HandleAppRunnerStop);
            Runners.Add(runner);
            _ = runner.Run();

            return new AppRunnerHandler(runner);
        }

        private void HandleAppRunnerStop(AppRunner runner)
        {
            Runners.Remove(runner);
        }
    }
}
