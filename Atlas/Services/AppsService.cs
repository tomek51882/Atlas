using Atlas.Core.Apps;
using Atlas.Interfaces;
using Atlas.Interfaces.Apps;
using Atlas.Models.DTOs;

namespace Atlas.Services
{
    internal class AppsService : IAppsService
    {
        public List<IAppExecutable> Apps { get; private set; } = new List<IAppExecutable>();
        public Dictionary<IAppExecutable, IAppRunner> Runners { get; private set; } = new Dictionary<IAppExecutable, IAppRunner> { };

        public event Action OnAppListChange = delegate { };

        public void AddApp(IAppExecutable app)
        {
            if (Apps.Any(x => x.Name == app.Name))
            {
                return;
            }

            Apps.Add(app);
            OnAppListChange.Invoke();
        }

        public AppRunnerHandler? ExecuteApp(IAppExecutable app)
        {
            if (Runners.TryGetValue(app, out var appRunner))
            {
                if (appRunner is not null && appRunner.IsRunning)
                {
                    return null;
                }
                appRunner = new AppRunner(app);
            }
            else
            {
                appRunner = new AppRunner(app);
                Runners.Add(app, appRunner);
            }

            _ = appRunner.Run();

            return new AppRunnerHandler(app, appRunner);
        }
    }
}
