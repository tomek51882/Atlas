using Atlas.Attributes;
using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Models.DTOs;
using Atlas.Primitives;
using Atlas.Services;
using Atlas.Types;
using Microsoft.Build.Construction;

namespace Atlas.Components
{
    internal class FileExplorer : ComponentBase
    {
        private Rect TempSolutionForUnknmownParentSize = new Rect(0, 0, 80, 19);
        private string currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..");

        private SelectContainer<SimpleFileInfo> FileList { get; set; } = new SelectContainer<SimpleFileInfo>();

        [Inject] private IWindowService? WindowService { get; init; }
        [Inject] private IDialogService DialogService { get; init; }

        public override void OnInitialized()
        {
            FileList.Rect = TempSolutionForUnknmownParentSize;
            FileList.RowTemplate = (item, row) => {
                row.Add(new Text(item.Name));
                row.Add(new RowSpacer());
            };
            LoadFiles();
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(FileList);
        }

        public override void OnKeyPressed(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                FileList?.SelectNext();
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                FileList?.SelectPrevious();
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                HandleSelection(FileList.SelectedValue);
            }
        }

        private async Task HandleSelection(SimpleFileInfo? data)
        {
            if (data is null)
            {
                return;
            }
            if (data.IsDirectory)
            {
                currentDir = Path.Combine(currentDir, data.Name);
                LoadFiles();
                return;
            }

            if (data.Name.EndsWith(".sln"))
            {
                
                var solution = Microsoft.Build.Construction.SolutionFile.Parse(Path.Combine(currentDir, data.Name));

                //if (solution.ProjectsInOrder.Count == 1)
                //{ 
                //}

                var popupParams = new PopupParameters<ProjectSelector>
                {
                    { x => x.Solution, solution }
                };

                var test = await DialogService.ShowDialogAsync<ProjectSelector>("Select Project", popupParams);
                var selectedProject = test.Data as ProjectInSolution;

            }
        }

        private void LoadFiles()
        {
            FileList.ClearList();
            var directories = Directory.GetDirectories(currentDir).Select(dir => new DirectoryInfo(dir)).OrderBy(dir => dir.Name).ToList();
            var files = Directory.GetFiles(currentDir).Select(file => new FileInfo(file)).OrderBy(file => file.Name).ToList();

            FileList.AddOption(new SimpleFileInfo { Name = "..", IsDirectory = true });
            FileList.AddOption(new SimpleFileInfo { Name = ".", IsDirectory = true });

            foreach (var dirInfo in directories)
            {
                FileList.AddOption(new SimpleFileInfo { Name = dirInfo.Name, IsDirectory = true });
            }

            foreach (var fileInfo in files)
            {
                FileList.AddOption(new SimpleFileInfo { Name = fileInfo.Name, IsDirectory = false, Size = fileInfo.Length });
            }
            
        }
    }
}
