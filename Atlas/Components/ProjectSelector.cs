using Atlas.Core;
using Atlas.Interfaces.Renderables;
using Atlas.Primitives;
using Microsoft.Build.Construction;

namespace Atlas.Components
{
    //parent  40, 8
    internal class ProjectSelector : ComponentBase
    {
        private SelectContainer<ProjectInSolution> projectList;
        public SolutionFile Solution { get; set; }


        public override void OnInitialized()
        {
            projectList = new SelectContainer<ProjectInSolution>();
            projectList.Rect = new Types.Rect(0, 0, 38, 6);

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
    }
}
