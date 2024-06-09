using Atlas.Attributes;
using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Interfaces.Apps;
using Atlas.Models.DTOs;
using Atlas.Primitives;
using Atlas.Services;
using Atlas.Types;
using Microsoft.Build.Construction;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Atlas.Components
{
    internal class FileExplorer : ComponentBase
    {
        private string currentDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..");

        private SelectContainer<SimpleFileInfo> FileList { get; set; } = new SelectContainer<SimpleFileInfo>();

        [Inject] private IDialogService DialogService { get; init; }
        [Inject] private IAppsService AppService { get; init; }

        public override void OnInitialized()
        {
            FileList.RowTemplate = (item, row) =>
            {
                row.Add(new Text(item.Name));
                row.Add(new RowSpacer());
            };
            FileList.StyleProperties.Width = new Core.Styles.StyleProperty<UnitValue<int>>(new UnitValue<int>(100, Unit.Percent));
            FileList.StyleProperties.Height = new Core.Styles.StyleProperty<UnitValue<int>>(new UnitValue<int>(100, Unit.Percent));
            LoadFiles();
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(FileList);
        }

        public override async void OnKeyPressed(ConsoleKeyInfo keyInfo)
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
                await HandleSelection(FileList.SelectedValue);
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
                await HandleSolutionFile(data);
            }
        }

        private async Task HandleSolutionFile(SimpleFileInfo data)
        {
            var solution = Microsoft.Build.Construction.SolutionFile.Parse(Path.Combine(currentDir, data.Name));
            var appSrv = Unsafe.As<AppsService>(AppService);

            if (solution.ProjectsInOrder.Count == 1)
            {
                appSrv.AddApp(new MSBuildApp(solution.ProjectsInOrder.First()));
                return;
            }

            var popupParams = new PopupParameters<ProjectSelector>
            {
                { x => x.Solution, solution }
            };

            var dialogResult = await DialogService.ShowDialogAsync<ProjectSelector>("Select Project", popupParams);
            var selectedProject = dialogResult.Data as ProjectInSolution;
            if (selectedProject is null)
            {
                return;
            }
            appSrv.AddApp(new MSBuildApp(selectedProject));
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
                FileList.AddOption(new SimpleFileInfo { Name = fileInfo.Name, IsDirectory = false, Size = fileInfo.Length, FullPath = fileInfo.FullName });
            }

        }
    }
}
