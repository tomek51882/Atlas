using Atlas.Interfaces;

namespace Atlas.Services
{
    internal class WindowService : IWindowService
    {
        private readonly IRenderer _renderer;

        public WindowService(IRenderer renderer)
        {
            this._renderer = renderer;
        }

        //internal void CreateWindow<T>() where T: BaseComponent, new()
        //{
        //    if(FocusedWindow is null) 
        //    {
        //        FocusedWindow = new Window();
        //        _renderer.EnqueueRender(FocusedWindow);
        //    }
        //    //var component = new T();

        //}
    }
}
