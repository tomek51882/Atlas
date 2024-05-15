using Atlas.Core;
using Atlas.Primitives;

namespace Atlas.Components
{
    internal class TestListComponent : ComponentBase
    {
        class TestData
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        private ListContainer<TestData> myList;


        public override void OnInitialized()
        {
            myList = new ListContainer<TestData>();
            myList.Rect = new Types.Rect(0, 0, 38, 8);

            myList.RowTemplate = (item, row) =>
            {
                row.Add(new Text(item.Name));
                row.Add(new RowSpacer());
                row.Add(new Text(item.Value));
            };

            myList.AddElement(new TestData { Name = "Item 1", Value = "Value 1" });
            myList.AddElement(new TestData { Name = "Item 2", Value = "Value 2" });
            myList.AddElement(new TestData { Name = "Item 3", Value = "Value 3" });


        }
        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(myList);
        }
    }
}
