using Atlas.Exceptions;
using Atlas.Interfaces.Apps;
using System.Diagnostics;

namespace Atlas.Core.Apps
{
    internal class AppRunner : IAppRunner
    {
        private RunnerStatus _runnerStatus = RunnerStatus.Created;
        private readonly List<string> logs = new List<string>();
        private Process? RunnerProcess;
        public Exception? Exception { get; private set; }
        public IAppExecutable App { get; init; }
        public RunnerStatus RunnerStatus
        {
            get
            {
                return _runnerStatus;
            }
            set
            {
                _runnerStatus = value;
                OnRunnerStatusChange(value);
            }
        }

        public event Action<AppRunner> OnRunnerStopped = delegate { };
        public event Action<RunnerStatus> OnRunnerStatusChange = delegate { };

        internal AppRunner(IAppExecutable app, Action<AppRunner> OnRunnerStopped)
        {
            App = app;
            logs.Add($"Runner for {app.Name} is created");
            RunnerStatus = RunnerStatus.WaitingToRun;
            this.OnRunnerStopped = OnRunnerStopped;
        }

        internal async Task Run()
        {
            await Task.Delay(100);
            logs.Add($"Runner activated");
            RunnerStatus = RunnerStatus.PreparingToRun;
            Dictionary<ExecutionCommandStage, ExecutionCommand?> commands = new Dictionary<ExecutionCommandStage, ExecutionCommand?>
            {
                { ExecutionCommandStage.PreBuild, App.PreBuild() },
                { ExecutionCommandStage.Build, App.Build() },
                { ExecutionCommandStage.PostBuild, App.PostBuild() },
                { ExecutionCommandStage.Run, App.Run() }
            };

            try
            {
                foreach (var kvp in commands)
                {
                    logs.Add($"Executing {kvp.Key} hook");
                    if (kvp.Key == ExecutionCommandStage.Build)
                    {
                        RunnerStatus = RunnerStatus.Building;
                    }
                    if (kvp.Key == ExecutionCommandStage.Run)
                    {
                        RunnerStatus = RunnerStatus.Running;
                    }
                    if (kvp.Value.HasValue == false)
                    {
                        logs.Add($"Nothing to do");
                        continue;
                    }

                    await ExecuteCommand(kvp.Value.Value);
                }
                RunnerStatus = RunnerStatus.RanToCompletion;
            }
            catch (Exception ex)
            {
                Exception = ex;
                RunnerStatus = RunnerStatus.Faulted;
            }

            logs.Add($"Runner stopped");
            OnRunnerStopped.Invoke(this);
        }
        private async Task ExecuteCommand(ExecutionCommand command)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = command.command,
                Arguments = command.arguments,
                UseShellExecute = true,
                CreateNoWindow = false
            };
            RunnerProcess = new Process { StartInfo = processInfo };
            logs.Add($"Executing {command.command} Args: {command.arguments}");
            RunnerProcess.Start();
            try
            {
                await RunnerProcess.WaitForExitAsync();
                int exitCode = RunnerProcess.ExitCode;

                if (exitCode != 0)
                {
                    throw new ProcessExecutionException(exitCode);
                }
                logs.Add($"Success - Process exited with code: {exitCode}");
            }
            finally
            {
                RunnerProcess.Dispose();
                RunnerProcess = null;
            }
        }

    }
    internal enum RunnerStatus
    {
        Created,
        WaitingToRun,
        PreparingToRun,
        Building,
        Running,
        RanToCompletion,
        Canceled,
        Faulted
    }
}
