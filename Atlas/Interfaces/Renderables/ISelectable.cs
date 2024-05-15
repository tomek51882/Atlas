
namespace Atlas.Interfaces.Renderables
{
    internal interface ISelectable
    {
        IPrimitive? SelectedItem { get; }
        void SelectNext();
        void SelectPrevious();
    }
}
