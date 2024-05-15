
using Atlas.Core;
using Atlas.Primitives;

namespace Atlas.Components
{
    internal class TestSelectComponent : ComponentBase
    {
        class TestData
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private SelectContainer<TestData> mySelect = new SelectContainer<TestData>();

        public override void OnInitialized()
        {
            mySelect.Rect = new Types.Rect(0, 0, 38, 8);

            mySelect.RowTemplate = (item, row) =>
            {
                row.Add(new Text(item.Name));
                row.Add(new RowSpacer());
                row.Add(new Text(item.Value));
            };

            mySelect.AddOption(new TestData { Name = "Item 1", Value = "Value 1" });
            mySelect.AddOption(new TestData { Name = "Item 2", Value = "Value 2" });
            mySelect.AddOption(new TestData { Name = "Item 3", Value = "Value 3" });
            mySelect.AddOption(new TestData { Name = "Item 4", Value = "Value 4" });
            mySelect.AddOption(new TestData { Name = "Item 5", Value = "Value 5" });
            mySelect.AddOption(new TestData { Name = "Item 6", Value = "Value 6" });
            mySelect.AddOption(new TestData { Name = "Item 7", Value = "Value 7" });
            mySelect.AddOption(new TestData { Name = "Item 8", Value = "Value 8" });
            mySelect.AddOption(new TestData { Name = "Item 9", Value = "Value 9" });
            mySelect.AddOption(new TestData { Name = "Item 10", Value = "Value 10" });
            mySelect.AddOption(new TestData { Name = "Item 11", Value = "Value 11" });
            mySelect.AddOption(new TestData { Name = "Item 12", Value = "Value 12" });
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(mySelect);
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                mySelect.SelectNext();
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                mySelect.SelectPrevious();
            }
        }

    }
}
