using MultiPublish.Publishing;

namespace MultiPublish.Tests
{
    public class DotnetPublishExecutorTests
    {
        private class FakeRunner : IDotnetProcessRunner
        {
            public List<IReadOnlyList<string>> CapturedArgs { get; } = new List<IReadOnlyList<string>>();

            public int ExitCodeToReturn { get; set; } = 0;

            public int Run(IReadOnlyList<string> arguments, out string standardOutput, out string standardError)
            {
                this.CapturedArgs.Add(arguments);
                standardOutput = "ok";
                standardError = string.Empty;
                return this.ExitCodeToReturn;
            }
        }

        [Fact]
        public void Execute_RunsDotnetPublish_WithGivenArgs()
        {
            FakeRunner runner = new FakeRunner();
            DotnetPublishExecutor executor = new DotnetPublishExecutor(runner);

            List<string> args = new List<string> { "-c", "Release", "-r", "win-x64" };

            DotnetPublishResult result = executor.Execute(args);

            Assert.True(result.Succeeded);
            Assert.Single(runner.CapturedArgs);
            Assert.Contains("publish", runner.CapturedArgs[0]);
            Assert.Contains("-c", runner.CapturedArgs[0]);
            Assert.Contains("win-x64", runner.CapturedArgs[0]);
        }

        [Fact]
        public void Execute_Failure_WhenExitCodeNonZero()
        {
            FakeRunner runner = new FakeRunner();
            runner.ExitCodeToReturn = 1;
            DotnetPublishExecutor executor = new DotnetPublishExecutor(runner);

            List<string> args = new List<string>();

            DotnetPublishResult result = executor.Execute(args);

            Assert.False(result.Succeeded);
        }
    }
}

