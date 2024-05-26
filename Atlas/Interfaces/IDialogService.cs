using Atlas.Interfaces.Renderables;
using Atlas.Services;

namespace Atlas.Interfaces
{
    internal interface IDialogService
    {
        void OpenPopup<T>(string popupTitle, PopupParameters<T> parameters) where T : IComponent, new();
        Task<DialogResult> ShowDialogAsync<T>(string popupTitle, PopupParameters<T> parameters) where T : IComponent, new();
        //void Close(PopupResult result);
    }
}
