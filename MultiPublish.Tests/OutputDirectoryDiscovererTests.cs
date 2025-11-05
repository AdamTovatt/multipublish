using MultiPublish.Abstractions;
using MultiPublish.Services;

namespace MultiPublish.Tests
{
    public class OutputDirectoryDiscovererTests
    {
        [Fact]
        public void TryGetOutputDirectoryFromArgs_FindsShortOption()
        {
            IOutputDirectoryDiscoverer d = new OutputDirectoryDiscoverer();
            List<string> args = new List<string> { "-o", "C:/out" };
            string? result = d.TryGetOutputDirectoryFromArgs(args);
            Assert.Equal("C:/out", result);
        }

        [Fact]
        public void TryGetOutputDirectoryFromArgs_FindsLongOption()
        {
            IOutputDirectoryDiscoverer d = new OutputDirectoryDiscoverer();
            List<string> args = new List<string> { "--output", "D:/publish" };
            string? result = d.TryGetOutputDirectoryFromArgs(args);
            Assert.Equal("D:/publish", result);
        }
    }
}

