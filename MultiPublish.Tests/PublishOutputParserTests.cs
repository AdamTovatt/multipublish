using MultiPublish.Abstractions;
using MultiPublish.Publish;
using MultiPublish.Services;

namespace MultiPublish.Tests
{
    public class PublishOutputParserTests
    {
        [Fact]
        public void ExtractPublishDirectory_ParsesOutputLine()
        {
            IPublishOutputParser parser = new PublishOutputParser();
            string output = "MinerUHost -> C:\\Users\\Adam\\code\\MinerUHost\\MinerUHost\\bin\\Release\\net8.0\\win-x86\\publish\\\n";
            DotnetPublishResult result = new DotnetPublishResult(true, 0, output, string.Empty);

            string? publishDir = parser.ExtractPublishDirectory(result);

            Assert.NotNull(publishDir);
            Assert.Equal("C:\\Users\\Adam\\code\\MinerUHost\\MinerUHost\\bin\\Release\\net8.0\\win-x86\\publish", publishDir);
        }

        [Fact]
        public void ExtractPublishDirectory_ParsesLinuxPath()
        {
            IPublishOutputParser parser = new PublishOutputParser();
            string output = "MyProject -> /home/user/project/bin/Release/net8.0/linux-x64/publish/\n";
            DotnetPublishResult result = new DotnetPublishResult(true, 0, output, string.Empty);

            string? publishDir = parser.ExtractPublishDirectory(result);

            Assert.NotNull(publishDir);
            Assert.Equal("/home/user/project/bin/Release/net8.0/linux-x64/publish", publishDir);
        }

        [Fact]
        public void ExtractPublishDirectory_ParsesFullMultiLineOutput()
        {
            IPublishOutputParser parser = new PublishOutputParser();
            string output = @"  Determining projects to restore...

  Restored C:\Users\Adam\code\MinerUHost\MinerUHost\MinerUHost.csproj (in 251 ms).

  Restored C:\Users\Adam\code\MinerUHost\MinerUHost.Tests\MinerUHost.Tests.csproj (in 350 ms).

  MinerUHost -> C:\Users\Adam\code\MinerUHost\MinerUHost\bin\Release\net8.0\win-x86\mineru-host.dll

  Successfully created package 'C:\Users\Adam\code\MinerUHost\MinerUHost\bin\Release\MinerUHost.0.1.1.nupkg'.

  Successfully created package 'C:\Users\Adam\code\MinerUHost\MinerUHost\bin\Release\MinerUHost.0.1.1.snupkg'.

  MinerUHost -> C:\Users\Adam\code\MinerUHost\MinerUHost\bin\Release\net8.0\win-x86\publish\
";
            DotnetPublishResult result = new DotnetPublishResult(true, 0, output, string.Empty);

            string? publishDir = parser.ExtractPublishDirectory(result);

            Assert.NotNull(publishDir);
            Assert.Equal("C:\\Users\\Adam\\code\\MinerUHost\\MinerUHost\\bin\\Release\\net8.0\\win-x86\\publish", publishDir);
        }

        [Fact]
        public void ExtractPublishDirectory_ReturnsNull_WhenNoMatch()
        {
            IPublishOutputParser parser = new PublishOutputParser();
            DotnetPublishResult result = new DotnetPublishResult(true, 0, "Some other output", string.Empty);

            string? publishDir = parser.ExtractPublishDirectory(result);

            Assert.Null(publishDir);
        }
    }
}

