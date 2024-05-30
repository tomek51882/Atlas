using Atlas.Core.Apps;
using Atlas.Database;
using Atlas.Interfaces;
using Atlas.Interfaces.Apps;
using Atlas.Models.Database;
using Atlas.Models.DTOs;

namespace Atlas.Services
{
    internal class AppsService : IAppsService
    {
        public List<IAppExecutable> Apps { get; private set; } = new List<IAppExecutable>();
        public Dictionary<IAppExecutable, IAppRunner> Runners { get; private set; } = new Dictionary<IAppExecutable, IAppRunner> { };

        public event Action OnAppListChange = delegate { };


        public AppsService()
        {
            RestoreFromDB();
        }

        private void RestoreFromDB()
        {
            using (var db = new DatabaseContext())
            {
                var apps = db.Apps.ToList();

                foreach (var app in apps)
                {
                    if (app.AppType == Types.AppType.MSBuildApp)
                    {
                        Apps.Add(new MSBuildApp { 
                            Name =  app.Name, 
                            AppType = app.AppType,
                            ExecutablePath = app.AppData
                        });
                    }
                }
            }
        }

        public void AddApp(IAppExecutable app)
        {
            if (Apps.Any(x => x.Name == app.Name))
            {
                return;
            }

            using (var db = new DatabaseContext())
            {
                var saveApp = new SavedAppsDb();
                saveApp.Name = app.Name;
                saveApp.AppType = app.AppType;
                saveApp.AppData = app.ExecutablePath;
                db.Apps.Add(saveApp);
                db.SaveChanges();
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
