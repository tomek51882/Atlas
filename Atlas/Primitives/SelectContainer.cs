
using Atlas.Core.Styles;
using Atlas.Extensions;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using System.Runtime.CompilerServices;

namespace Atlas.Primitives
{
    internal class SelectContainer<T> : IPrimitive, IRenderableContainer, ISelectable, IScrollable where T : class
    {
        private Action<T, SelectItem<T>> _rowTemplate = (item, row) => row.Children.Add(new Text(item?.ToString()));
        private Dictionary<T, SelectItem<T>> itemsLookup = new Dictionary<T, SelectItem<T>>();
        private Rect _rect = new Rect();

        public T? SelectedValue { get; set; }
        public IPrimitive? SelectedItem { get; set; }
        public Rect Rect
        {
            get
            {
                return _rect;
            }
            set
            {
                _rect = value;
                if (Children.Count > 0)
                {
                    UpdateChildrenSize();
                }
            }
        }
        public List<IRenderable> Children { get; set; } = new List<IRenderable>();
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();
        public Vector2Int ScrollOffset { get; set; }
        public Action<T, SelectItem<T>> RowTemplate
        {
            get => _rowTemplate;
            set
            {
                _rowTemplate = value;
                RefreshOptions();
            }
        }
        public void RefreshOptions()
        {
            Children.ForEach(x =>
            {
                //Should be fine since all items of list should be of type SelectItem
                var listItem = Unsafe.As<SelectItem<T>>(x);
                var value = listItem.Value;
                listItem.ClearContent();
                RowTemplate(value, listItem);
                listItem.RecalculateLayout();
            });
        }
        public List<IRenderable> ChildrenInRect
        {
            get
            {
                for (int i = 0; i < this.Rect.height && i < this.Children.Count; i++)
                {

                }
                return Children;
            }
        }
        public void ClearList()
        {
            SelectedValue = default;
            SelectedItem = null;
            ScrollOffset = Vector2Int.Zero;
            itemsLookup.Clear();
            Children.Clear();
        }
        public void AddOption(T child)
        {
            if (child is null)
            {
                return;
            }

            var item = new SelectItem<T>(child);
            item.Rect = new Rect(0, Children.Count, Rect.width, 1);
            RowTemplate(child, item);
            item.RecalculateLayout();

            if (SelectedItem is null)
            {
                SelectedValue = item.Value;
                SelectedItem = item;
            }
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
        public void SelectNext()
        {
            if (SelectedItem is null || Children.Count == 0)
            {
                return;
            }

            int index = Math.Clamp(Children.IndexOf(SelectedItem) + 1, 0, Children.Count - 1);
            var newSelectedItem = (SelectItem<T>)Children[index];
            if (SelectedItem == newSelectedItem)
            {
                return;
            }
            SelectedValue = newSelectedItem.Value;
            SelectedItem = newSelectedItem;

            if (SelectedItem.Rect.y - ScrollOffset.y >= this.Rect.height)
            {
                ScrollDown();
            }
        }
        public void SelectPrevious()
        {
            if (SelectedItem is null || Children.Count == 0)
            {
                return;
            }

            int index = Math.Clamp(Children.IndexOf(SelectedItem) - 1, 0, Children.Count - 1);
            var newSelectedItem = (SelectItem<T>)Children[index];
            if (SelectedItem == newSelectedItem)
            {
                return;
            }
            SelectedValue = newSelectedItem.Value;
            SelectedItem = newSelectedItem;

            if (SelectedItem.Rect.y < 0)
            {
                ScrollUp();
            }
        }
        public void ScrollUp()
        {
            this.ScrollOffset = this.ScrollOffset += new Vector2Int(0, 1);
            ApplyScroll();
        }
        public void ScrollDown()
        {
            this.ScrollOffset = this.ScrollOffset += new Vector2Int(0, -1);
            ApplyScroll();
        }
        private void ApplyScroll()
        {
            SelectItem<T> item;
            for (int i = 0; i < Children.Count; i++)
            {
                item = Unsafe.As<SelectItem<T>>(Children[i]);
                item.ScrollOffset = this.ScrollOffset;
            }
        }
        private void UpdateChildrenSize()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var childPrimitive = (IPrimitive)Children[i];
                childPrimitive.Rect = new Rect(0, i, Rect.width, 1);
            }
        }
    }

    class SelectItem<T> : RowLayout, ISelectableItem
    {
        public T Value { get; set; }
        public Vector2Int ScrollOffset { get; set; }
        public override Rect Rect
        {
            get
            {
                return base.Rect.Move(ScrollOffset);
            }
            set
            {
                base.Rect = value;
                RecalculateLayout();
            }
        }
        public SelectItem(T value)
        {
            Value = value;
        }
    }
}
