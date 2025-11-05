using MultiPublish.ArgParsing;
using MultiPublish.Publish;

namespace MultiPublish.Tests
{
    public class PublishCommandGeneratorTests
    {
        [Fact]
        public void GenerateConfigurations_CartesianProduct()
        {
            ParsedArguments parsed = new ParsedArguments(
                new List<string> { "win-x64", "osx-arm64" },
                new List<bool> { true, false },
                new List<string> { "-c", "Release" },
                true
            );

            IReadOnlyList<PublishConfiguration> configs = PublishCommandGenerator.GenerateConfigurations(parsed);

            Assert.Equal(4, configs.Count);
            Assert.Contains(configs, c => c.Runtime == "win-x64" && c.SelfContained == true);
            Assert.Contains(configs, c => c.Runtime == "win-x64" && c.SelfContained == false);
            Assert.Contains(configs, c => c.Runtime == "osx-arm64" && c.SelfContained == true);
            Assert.Contains(configs, c => c.Runtime == "osx-arm64" && c.SelfContained == false);
        }

        [Fact]
        public void GenerateConfigurations_Defaults_WhenNoneSpecified()
        {
            ParsedArguments parsed = new ParsedArguments(
                new List<string>(),
                new List<bool>(),
                new List<string>(),
                true
            );

            IReadOnlyList<PublishConfiguration> configs = PublishCommandGenerator.GenerateConfigurations(parsed);

            Assert.Single(configs);
            Assert.Null(configs[0].Runtime);
            Assert.Null(configs[0].SelfContained);
        }

        [Fact]
        public void BuildCommand_IncludesFlags_AccordingToConfiguration()
        {
            PublishConfiguration config = new PublishConfiguration("win-x64", true);
            List<string> baseArgs = new List<string> { "-c", "Release", "--nologo" };

            IReadOnlyList<string> commandArgs = PublishCommandGenerator.BuildCommandArgs(baseArgs, config);

            Assert.Contains("-r", commandArgs);
            Assert.Contains("win-x64", commandArgs);
            Assert.Contains("--self-contained", commandArgs);
            Assert.DoesNotContain("--no-self-contained", commandArgs);
        }

        [Fact]
        public void BuildCommand_UsesNoSelfContained_WhenFalse()
        {
            PublishConfiguration config = new PublishConfiguration("linux-x64", false);
            List<string> baseArgs = new List<string>();

            IReadOnlyList<string> commandArgs = PublishCommandGenerator.BuildCommandArgs(baseArgs, config);

            Assert.Contains("-r", commandArgs);
            Assert.Contains("linux-x64", commandArgs);
            Assert.Contains("--no-self-contained", commandArgs);
        }

        [Fact]
        public void BuildCommand_OmitsRuntime_WhenNull()
        {
            PublishConfiguration config = new PublishConfiguration(null, true);
            List<string> baseArgs = new List<string>();

            IReadOnlyList<string> commandArgs = PublishCommandGenerator.BuildCommandArgs(baseArgs, config);

            Assert.DoesNotContain("-r", commandArgs);
            Assert.Contains("--self-contained", commandArgs);
        }
    }
}

