using MultiPublish.Abstractions;
using MultiPublish.Publish;

namespace MultiPublish.Services
{
    public class PublishDirectoryFinder : IPublishDirectoryFinder
    {
        public string? FindPublishDirectory(PublishConfiguration configuration, IReadOnlyList<string> publishArgs, string workingDirectoryPath)
        {
            string? configurationValueNullable = this.GetOptionValue(publishArgs, "-c", "--configuration");
            string configurationValue = string.IsNullOrEmpty(configurationValueNullable) ? "Debug" : configurationValueNullable;

            string? frameworkValue = this.GetOptionValue(publishArgs, "-f", "--framework");

            string binRoot = Path.Combine(workingDirectoryPath, "bin");
            if (!Directory.Exists(binRoot))
            {
                return null;
            }

            IEnumerable<string> candidates = Directory.EnumerateDirectories(binRoot, "*", SearchOption.AllDirectories);
            List<(string Path, DateTime Modified)> publishDirs = new List<(string, DateTime)>();
            foreach (string dir in candidates)
            {
                if (string.Equals(Path.GetFileName(dir), "publish", StringComparison.OrdinalIgnoreCase))
                {
                    if (!dir.Contains(Path.DirectorySeparatorChar + configurationValue + Path.DirectorySeparatorChar))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(frameworkValue) && !dir.Contains(Path.DirectorySeparatorChar + frameworkValue + Path.DirectorySeparatorChar))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(configuration.Runtime) && !dir.Contains(Path.DirectorySeparatorChar + configuration.Runtime + Path.DirectorySeparatorChar))
                    {
                        continue;
                    }

                    DateTime lastWrite = Directory.GetLastWriteTimeUtc(dir);
                    publishDirs.Add((dir, lastWrite));
                }
            }

            if (publishDirs.Count == 0)
            {
                return null;
            }

            publishDirs.Sort((a, b) => b.Modified.CompareTo(a.Modified));
            return publishDirs[0].Path;
        }

        private string? GetOptionValue(IReadOnlyList<string> args, string shortName, string longName)
        {
            for (int i = 0; i < args.Count; i += 1)
            {
                string current = args[i];
                if (current == shortName || current == longName)
                {
                    if (i + 1 < args.Count)
                    {
                        return args[i + 1];
                    }
                }
            }

            return null;
        }
    }
}

