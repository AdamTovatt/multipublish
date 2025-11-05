using MultiPublish.Services;

namespace MultiPublish.Tests
{
    public class BinDirectoryResolverTests
    {
        [Fact]
        public void ResolveBinDirectoryFromPublish_WalksUpToBin()
        {
            BinDirectoryResolver r = new BinDirectoryResolver();
            string inputPath = Path.Combine("C:", "repo", "bin", "Release", "net8.0", "win-x64", "publish");
            string fallback = Path.Combine("C:", "repo");
            string path = r.ResolveBinDirectoryFromPublish(inputPath, fallback);
            string expected = Path.Combine("C:", "repo", "bin");
            Assert.Equal(expected, path);
        }
    }
}

