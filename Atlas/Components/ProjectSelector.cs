using Atlas.Attributes;
using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Primitives;
using Atlas.Services;
using Microsoft.Build.Construction;

namespace Atlas.Components
{
    //parent  40, 8
    internal class ProjectSelector : ComponentBase
    {
        private SelectContainer<ProjectInSolution> projectList;
        public SolutionFile Solution { get; set; }

        [Inject]
        private DialogReference DialogReference { get; init; }


        public override void OnInitialized()
        {
            projectList = new SelectContainer<ProjectInSolution>();
            projectList.Rect = new Types.Rect(0, 0, 38, 6);
            projectList.StyleProperties.ZIndex = new Core.Styles.StyleProperty<int>(20);

            projectList.RowTemplate = (item, row) =>
            {
                row.Add(new Text(item.ProjectName));
                row.Add(new RowSpacer());
            };

            foreach(var project in Solution.ProjectsInOrder)
            {
                projectList.AddOption(project);
            }
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(projectList);
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                var project = projectList.SelectedValue;
                DialogReference.Close(DialogResult.Ok(project));
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                projectList.SelectNext();
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                projectList.SelectPrevious();
            }
        }
    }
}