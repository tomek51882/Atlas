
using Microsoft.Build.Construction;

namespace Atlas.Interfaces.Apps
{
    internal class MSBuildApp : AppBase
    {
        private string PathToCsProjFile { get; set; }

        public override string ExecutablePath {
            get => PathToCsProjFile;
            set => PathToCsProjFile = value;
        }

        public MSBuildApp() { }
        public MSBuildApp(ProjectInSolution project) {
            AppType = Types.AppType.MSBuildApp;
            Name = project.ProjectName;
            PathToCsProjFile = project.AbsolutePath;
        }

        public override ExecutionCommand? Build()
        {
            return new ExecutionCommand
            {
                command = "dotnet",
                arguments = $"build {PathToCsProjFile}"
            };
            //ExecutableArguments = $"build --project {PathToCsProjFile}";
        }

        public override ExecutionCommand? Run()
        {
            return new ExecutionCommand
            {
                command = "dotnet",
                arguments = $"run --project {PathToCsProjFile}"
            };
        }
    }
}
