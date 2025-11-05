using MultiPublish.Publish;
using System.IO.Compression;

namespace MultiPublish.Tests
{
    public class PublishOutputZipperTests
    {
        [Fact]
        public void CreateZip_CreatesZipWithExpectedName()
        {
            string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);

            try
            {
                string publishDir = Path.Combine(tempRoot, "publish");
                Directory.CreateDirectory(publishDir);
                File.WriteAllText(Path.Combine(publishDir, "app.dll"), "content");

                string binDir = Path.Combine(tempRoot, "bin");
                Directory.CreateDirectory(binDir);

                PublishConfiguration config = new PublishConfiguration("win-x64", true);

                string zipPath = PublishOutputZipper.CreateZip(
                    projectName: "MinerUHost",
                    binDirectoryPath: binDir,
                    publishDirectoryPath: publishDir,
                    configuration: config
                );

                Assert.True(File.Exists(zipPath));
                string expectedName = "MinerUHost-win-x64-self-contained.zip";
                Assert.Equal(expectedName, Path.GetFileName(zipPath));

                // Ensure it is a valid zip with our file inside
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    Assert.NotNull(archive.GetEntry("app.dll"));
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

        [Fact]
        public void CreateZip_FrameworkDependent_UsesFrameworkDependentInName()
        {
            string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);

            try
            {
                string publishDir = Path.Combine(tempRoot, "publish");
                Directory.CreateDirectory(publishDir);
                File.WriteAllText(Path.Combine(publishDir, "app.dll"), "content");

                string binDir = Path.Combine(tempRoot, "bin");
                Directory.CreateDirectory(binDir);

                PublishConfiguration config = new PublishConfiguration("linux-x64", false);

                string zipPath = PublishOutputZipper.CreateZip(
                    projectName: "MinerUHost",
                    binDirectoryPath: binDir,
                    publishDirectoryPath: publishDir,
                    configuration: config
                );

                string expectedName = "MinerUHost-linux-x64-framework-dependent.zip";
                Assert.Equal(expectedName, Path.GetFileName(zipPath));
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

