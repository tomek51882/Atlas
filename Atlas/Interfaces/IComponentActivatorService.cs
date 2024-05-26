namespace Atlas.Interfaces
{
    internal interface IComponentActivatorService
    {
        void ResolveDependencies(object target);
        T? Inject<T>(object target) where T : new();
        void Inject<T>(object target, T injectionSource) where T : new();
    }
}