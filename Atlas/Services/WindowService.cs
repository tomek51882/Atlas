using Atlas.Attributes;
using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Interfaces;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using Atlas.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Atlas.Services
{
    internal class WindowService : IWindowService
    {
        private readonly IRenderer renderer;
        private readonly IInputSystem inputSystem;
        private readonly InputSystemDebugLine inputSystemDebugLine;
        private readonly IServiceProvider serviceProvider;

        private Window? FocusedWindow;
        private Dictionary<string, Window> OpenedWindows { get; set; } = new Dictionary<string, Window>();

        public WindowService(IRenderer renderer, IInputSystem inputSystem, IServiceProvider serviceProvider)
        {
            this.renderer = renderer;
            this.inputSystem = inputSystem;
            this.serviceProvider = serviceProvider;

            this.inputSystem.OnKeyPress += HandleKeyPress;
            (_, inputSystemDebugLine) = CreateWindow<InputSystemDebugLine>(new Types.Rect(0, 24, 126, 3), "Input Debug");
        }

        public (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle) where T : IComponent, new()
        {
            if(FocusedWindow is not null) 
            {
                FocusedWindow.IsFocused = false;
            }

            var component = new T(); // serviceProvider.GetRequiredService<T>();
            Inject(component);

            FocusedWindow = new Window(windowRect, component, ShortIdGenerator.Generate());
            FocusedWindow.IsFocused = true;
            FocusedWindow.Title = windowTitle;
            FocusedWindow.StyleProperties.Padding = new Core.Styles.PaddingProperty(1);
            OpenedWindows.Add(FocusedWindow.WindowId, FocusedWindow);
            Unsafe.As<Renderer>(renderer).MountRenderable(FocusedWindow);
            return (FocusedWindow.WindowId, component);
        }

        private void Inject(object target)
        { 
            var type = target.GetType();
            var properties = type
                .GetProperties(BindingFlags.Public 
                    | BindingFlags.NonPublic 
                    | BindingFlags.Instance)
                .Where(x=>x.IsDefined(typeof(InjectAttribute), false));

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var dependency = serviceProvider.GetService(propertyType);
                if (dependency is null)
                {
                    continue;
                }

                if (property.CanWrite)
                {
                    property.SetValue(target, dependency);
                    continue;
                }

                //Try setting the value by private setter
                var setMethod = property.GetSetMethod(true);
                if (setMethod != null)
                {
                    setMethod.Invoke(target, new object[] { dependency });
                }

                //Try accessing the backing field
                //NOTE: Maybe not needed?
            }
        }

        private void HandleKeyPress(ConsoleKeyInfo info)
        {
            FocusedWindow?.OnKeyPressed(info);
        }

        public void OpenPopup<T>(string popupTitle, PopupParameters<T> parameters) where T : IComponent, new()
        {
            (_, var component) = CreateWindow<T>(new Rect(50, 10, 40, 8), popupTitle);

            var targetType = component.GetType();

            foreach (var parameter in parameters)
            {
                PropertyInfo? property = targetType.GetProperty(parameter.Key, BindingFlags.Public | BindingFlags.Instance);

                if(property is not null && property.CanWrite)
                {
                    property.SetValue(component, parameter.Value);
                }
            }
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
