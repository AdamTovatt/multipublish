using MultiPublish.ArgParsing;
using MultiPublish.Publishing;

namespace MultiPublish.Tests
{
    public class EndToEndTests
    {
        [Fact]
        public void EndToEnd_GeneratesZip_ForSingleConfiguration()
        {
            string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);

            try
            {
                string publishDir = Path.Combine(tempRoot, "publish");
                Directory.CreateDirectory(publishDir);
                File.WriteAllText(Path.Combine(publishDir, "app.dll"), "content");

                ParsedArguments parsed = new ParsedArguments(
                    new List<string> { "win-x64" },
                    new List<bool> { true },
                    new List<string> { "-o", publishDir },
                    true
                );

                IReadOnlyList<PublishConfiguration> configs = PublishCommandGenerator.GenerateConfigurations(parsed);

                string binDir = Path.Combine(tempRoot, "bin");
                Directory.CreateDirectory(binDir);

                foreach (PublishConfiguration c in configs)
                {
                    string zip = PublishOutputZipper.CreateZip("SampleProject", binDir, publishDir, c);
                    Assert.True(File.Exists(zip));
                }
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }
    }
}

