
using Atlas.Attributes;
using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Interfaces.Apps;
using Atlas.Models.DTOs;
using Atlas.Primitives;
using Atlas.Services;
using System.Runtime.CompilerServices;

namespace Atlas.Components
{
    class AppItem
    {
        public IAppExecutable App { get; set; }
        public IAppRunner? Runner { get; set; }
        public Text Text { get; set; }
    }
    internal class AppListComponent : ComponentBase
    {
        [Inject]
        private IAppsService AppService { get; init; }

        private SelectContainer<AppItem> mySelect = new SelectContainer<AppItem>();

        public override void OnInitialized()
        {
            //mySelect.Rect = new Types.Rect(0, 0, 38, 8);

            mySelect.StyleProperties.Width = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            mySelect.StyleProperties.Height = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));

            mySelect.RowTemplate = (item, row) =>
            {
                row.Add(new Text(item.App.Name));
                row.Add(new RowSpacer());
                if (item.Runner?.RunnerStatus == Core.Apps.RunnerStatus.Building)
                {
                    row.Add(new ProgressSpinner());
                }
                if (item.Runner is not null)
                {
                    row.Add(new Text($"["));
                    row.Add(item.Text);
                    row.Add(new Text("] "));
                }
                row.Add(new Text(item.App.AppType.ToString()));
            };

            AppService.OnAppListChange += HandleAppListChange;
        }

        private void HandleAppListChange()
        {
            mySelect.ClearList();
            foreach (var app in AppService.Apps)
            {
                mySelect.AddOption(new AppItem { App = app, Text = new Text() });
            }
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
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                var app = mySelect.SelectedValue;
                if (app is null)
                {
                    return;
                }
                HandleApp(app);
            }
        }

        private void HandleApp(AppItem app)
        {
            var apps = Unsafe.As<AppsService>(AppService);

            var ah = apps.ExecuteApp(app.App);
            app.Runner = ah.AppRunner;
            app.Runner.OnRunnerStatusChange += (status) => 
            {

                if (status == Core.Apps.RunnerStatus.Running)
                {
                    app.Text.Value = "Running";
                    app.Text.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0x00ff00));
                }
                else if (status == Core.Apps.RunnerStatus.Faulted)
                {
                    app.Text.Value = "Failed";
                    app.Text.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0xff0000));
                }
                else
                {
                    app.Text.Value = status.ToString();
                    app.Text.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(Types.Color.DefaultForeground);
                }
                mySelect.RefreshOptions();
            };
        }

        private void Runner_OnRunnerStatusChange(Core.Apps.RunnerStatus obj)
        {
            mySelect.RefreshOptions();
        }
    }
}
