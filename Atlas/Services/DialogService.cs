using Atlas.Interfaces;
using Atlas.Types;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using Atlas.Interfaces.Renderables;

namespace Atlas.Services
{
    internal class DialogService : IDialogService
    {
        private readonly IWindowService _windowService;
        private readonly IComponentActivatorService _componentActivatorService;

        public DialogService(IWindowService windowService, IComponentActivatorService activatorService)
        {
            _windowService = windowService;
            _componentActivatorService = activatorService;
        }

        public void OpenPopup<T>(string popupTitle, PopupParameters<T> parameters) where T : IComponent, new()
        {
            (_, var component) = _windowService.CreateWindow<T>(new Rect(50, 10, 40, 8), popupTitle);

            var targetType = component.GetType();

            foreach (var parameter in parameters)
            {
                PropertyInfo? property = targetType.GetProperty(parameter.Key, BindingFlags.Public | BindingFlags.Instance);

                if (property is not null && property.CanWrite)
                {
                    property.SetValue(component, parameter.Value);
                }
            }
        }

        public Task<DialogResult> ShowDialogAsync<T>(string popupTitle, PopupParameters<T> parameters) where T : IComponent, new()
        {
            (var windowId, var component) = _windowService.CreateWindow<T>(new Rect(40, 8, 40, 8), popupTitle);

            var injectedReference = _componentActivatorService.Inject<DialogReference>(component);
            if (injectedReference is null)
            {
                throw new Exception("Failed to inject Dialog Reference");
            }

            var targetType = component.GetType();

            foreach (var parameter in parameters)
            {
                PropertyInfo? property = targetType.GetProperty(parameter.Key, BindingFlags.Public | BindingFlags.Instance);

                if (property is not null && property.CanWrite)
                {
                    property.SetValue(component, parameter.Value);
                }
            }

            var tcs = new TaskCompletionSource<DialogResult>();
            //injectedReference.WindowId = windowId;
            //injectedReference.IsOpen = true;

            injectedReference._close = (DialogResult res) =>
            {
                _windowService.CloseWindow(windowId);
                tcs.SetResult(res);
            };

            //return Task.FromResult((IDialogReference)injectedReference);
            return tcs.Task;
        }

        public void Close(DialogResult result)
        {

        }
    }
    class DialogReference // : IDialogReference
    {
        internal Action<DialogResult> _close = delegate { };
        //public PopupResult Result { get; private set; }

        public void Cancel()
        {
            _close?.Invoke(DialogResult.Cancel());
        }
        public void Close()
        {
            _close?.Invoke(DialogResult.Ok());
        }
        public void Close(DialogResult result)
        {
            _close?.Invoke(result);
        }
    }

    public class DialogResult
    {
        public object? Data { get; }
        public Type DataType { get; }
        public bool Canceled { get; }

        protected internal DialogResult(object? data, Type dataType, bool canceled)
        {
            Data = data;
            DataType = dataType;
            Canceled = canceled;
        }
        public static DialogResult Ok()
        {
            return Ok<object>(null);
        }
        public static DialogResult Ok<T>(T? result)
        {
            return new DialogResult(result, typeof(T), canceled: false);
        }

        public static DialogResult Cancel()
        {
            return new DialogResult(null, typeof(object), canceled: true);
        }

    }
    public class PopupParameters<T>() : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        private Dictionary<string, object?> paramDict = new Dictionary<string, object?>();

        internal void Add<TParam>(Expression<Func<T, TParam>> propertyExpression, TParam value)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new Exception("Lul, only member access is allowed");
            }

            Add(memberExpression.Member.Name, value);
        }

        private void Add(string name, object? value)
        {
            paramDict.Add(name, value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return paramDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return paramDict.GetEnumerator();
        }
    }
    public class PopupOptions
    {

    }

}
