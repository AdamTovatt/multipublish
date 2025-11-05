using System;
using System.IO;
using System.IO.Compression;

namespace MultiPublish.Publishing
{
    public static class PublishOutputZipper
    {
        public static string CreateZip(
            string projectName,
            string binDirectoryPath,
            string publishDirectoryPath,
            PublishConfiguration configuration)
        {
            string multipublishDir = Path.Combine(binDirectoryPath, "MultiPublish");
            Directory.CreateDirectory(multipublishDir);

            string runtimePart = string.IsNullOrEmpty(configuration.Runtime) ? "unknown" : configuration.Runtime!;
            string scPart = configuration.SelfContained.HasValue
                ? (configuration.SelfContained.Value ? "self-contained" : "framework-dependent")
                : "framework-dependent";

            string zipFileName = string.Format(
                "{0}-{1}-{2}.zip",
                projectName,
                runtimePart,
                scPart
            );

            string zipPath = Path.Combine(multipublishDir, zipFileName);

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(publishDirectoryPath, zipPath, CompressionLevel.Optimal, includeBaseDirectory: false);

            return zipPath;
        }
    }
}

