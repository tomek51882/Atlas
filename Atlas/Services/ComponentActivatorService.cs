using Atlas.Attributes;
using Atlas.Interfaces;
using Atlas.Interfaces.Renderables;
using System.Reflection;

namespace Atlas.Services
{
    internal class ComponentActivatorService : IComponentActivatorService
    {
        private readonly IServiceProvider serviceProvider;
        public ComponentActivatorService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void ResolveDependencies(object target) 
        {
            var type = target.GetType();
            var properties = type
                .GetProperties(BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false));

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
                setMethod?.Invoke(target, [dependency]);

                //Try accessing the backing field
                //NOTE: Maybe not needed?
            }
        }

        public T? Inject<T>(object target) where T : new()
        {
            var targetType = target.GetType();
            var properties = targetType
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false));

            var property = properties.Where(x=>x.PropertyType == typeof(T)).FirstOrDefault();

            if (property is null)
            {
                return default;
            }

            var dependency = new T();
            if (property.CanWrite)
            {
                property.SetValue(target, dependency);
                return dependency;
            }

            var setMethod = property.GetSetMethod(true);
            setMethod?.Invoke(target, [dependency]);
            return dependency;
        }
        public void Inject<T>(object target, T injectionSource) where T : new() 
        {
            var targetType = target.GetType();
            var properties = targetType
                .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance)
                .Where(x => x.IsDefined(typeof(InjectAttribute), false));

            var property = properties
                .FirstOrDefault(x => x.PropertyType == typeof(T));

            if (property is null)
            {
                return;
            }

            if (property.CanWrite)
            {
                property.SetValue(target, injectionSource);
                return;
            }

            var setMethod = property.GetSetMethod(true);
            setMethod?.Invoke(target, [injectionSource]);
        }
    }

}
