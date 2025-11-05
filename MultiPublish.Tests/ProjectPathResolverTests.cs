using MultiPublish.Abstractions;
using MultiPublish.Services;

namespace MultiPublish.Tests
{
    public class ProjectPathResolverTests
    {
        [Fact]
        public void ResolveProjectPath_ReturnsNull_WhenNoPath()
        {
            string[] args = new string[] { "-c", "Release", "-r", "win-x64" };
            IProjectPathResolver resolver = new ProjectPathResolver();

            string? result = resolver.ResolveProjectPath(args);

            Assert.Null(result);
        }

        [Fact]
        public void ResolveProjectPath_IgnoresBracketTokens()
        {
            string[] args = new string[] { "[win-x64,", "win-x86]", "--nologo" };
            IProjectPathResolver resolver = new ProjectPathResolver();

            string? result = resolver.ResolveProjectPath(args);

            Assert.Null(result);
        }

        [Fact]
        public void ResolveProjectPath_PicksCsproj()
        {
            string[] args = new string[] { "MinerUHost.csproj", "-c", "Release" };
            IProjectPathResolver resolver = new ProjectPathResolver();

            string? result = resolver.ResolveProjectPath(args);

            Assert.Equal("MinerUHost.csproj", result);
        }
    }
}

