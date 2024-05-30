using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using System.Runtime.CompilerServices;

namespace Atlas.Primitives
{
    internal class ListContainer<T> : IPrimitive, IRenderableContainer where T : class
    {
        private Action<T, ListItem<T>> _rowTemplate = (item, row) => row.Children.Add(new Text(item?.ToString()));
        private Dictionary<T, ListItem<T>> itemsLookup = new Dictionary<T, ListItem<T>>();

        public Action<T, ListItem<T>> RowTemplate
        {
            get => _rowTemplate;
            set
            {
                _rowTemplate = value;
                Children.ForEach(x =>
                {
                    //Should be fine since all items of list should be of type ListItem
                    var listItem = Unsafe.As<ListItem<T>>(x);
                    var value = listItem.Value;
                    listItem.ClearContent();
                    RowTemplate(value, listItem);
                    listItem.RecalculateLayout();
                });
            }
        }

        public Rect Rect { get; set; }
        public List<IRenderable> Children { get; set; } = new List<IRenderable>();
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();

        public void AddElement(T child)
        {
            if (child is null)
            {
                return;
            }

            var item = new ListItem<T>(child);
            item.Rect = new Rect(0, Children.Count, Rect.width, 1);
            RowTemplate(child, item);
            item.RecalculateLayout();


            itemsLookup.Add(child, item);
            Children.Add(item);
        }

        public void RemoveElement(T child)
        {
            if (itemsLookup.TryGetValue(child, out var listItem))
            {
                itemsLookup.Remove(child);
                Children.Remove(listItem);
            }
        }
    }

    class ListItem<T> : RowLayout
    {
        public T Value { get; set; }

        internal ListItem(T item)
        {
            Value = item;
        }
    }
}
