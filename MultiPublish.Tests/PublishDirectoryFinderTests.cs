using MultiPublish.Publishing;
using MultiPublish.Services;

namespace MultiPublish.Tests
{
    public class PublishDirectoryFinderTests
    {
        [Fact]
        public void FindPublishDirectory_ReturnsNull_WhenBinMissing()
        {
            PublishDirectoryFinder finder = new PublishDirectoryFinder();
            PublishConfiguration cfg = new PublishConfiguration("win-x64", true);
            List<string> args = new List<string> { "-c", "Release" };

            string? result = finder.FindPublishDirectory(cfg, args, "Z:/non-existent-root");

            Assert.Null(result);
        }
    }
}

