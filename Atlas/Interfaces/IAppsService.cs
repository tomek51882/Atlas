
using Atlas.Interfaces.Apps;
using Atlas.Models.DTOs;

namespace Atlas.Interfaces
{
    internal interface IAppsService
    {
        event Action OnAppListChange;

        List<IAppExecutable> Apps { get; }
    }
}
