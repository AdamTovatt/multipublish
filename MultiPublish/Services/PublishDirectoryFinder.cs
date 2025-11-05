using System;
using System.Linq;
using MultiPublish.Abstractions;
using MultiPublish.Publishing;

namespace MultiPublish.Services
{
    public class PublishDirectoryFinder : IPublishDirectoryFinder
    {
        public string? FindPublishDirectory(PublishConfiguration configuration, IReadOnlyList<string> publishArgs, string workingDirectoryPath)
        {
            string? configurationValueNullable = this.GetOptionValue(publishArgs, "-c", "--configuration");
            string configurationValue = string.IsNullOrEmpty(configurationValueNullable) ? "Debug" : configurationValueNullable;

            string? frameworkValue = this.GetOptionValue(publishArgs, "-f", "--framework");

            string searchRoot = this.GetSearchRoot(publishArgs, workingDirectoryPath);
            
            List<string> binRoots = new List<string>();
            string primaryBinRoot = Path.Combine(searchRoot, "bin");
            if (Directory.Exists(primaryBinRoot))
            {
                binRoots.Add(primaryBinRoot);
            }
            
            if (binRoots.Count == 0)
            {
                string workingBinRoot = Path.Combine(workingDirectoryPath, "bin");
                if (Directory.Exists(workingBinRoot))
                {
                    binRoots.Add(workingBinRoot);
                }
                else if (Directory.Exists(workingDirectoryPath))
                {
                    foreach (string subDir in Directory.EnumerateDirectories(workingDirectoryPath))
                    {
                        string subBinRoot = Path.Combine(subDir, "bin");
                        if (Directory.Exists(subBinRoot))
                        {
                            binRoots.Add(subBinRoot);
                        }
                    }
                }
            }
            
            if (binRoots.Count == 0)
            {
                return null;
            }

            IEnumerable<string> candidates = binRoots.SelectMany(binRoot => Directory.EnumerateDirectories(binRoot, "*", SearchOption.AllDirectories));
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

        private string GetSearchRoot(IReadOnlyList<string> publishArgs, string workingDirectoryPath)
        {
            string? projectPath = null;
            for (int i = 0; i < publishArgs.Count; i += 1)
            {
                string current = publishArgs[i];
                if (current.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
                    current.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
                {
                    projectPath = current;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(projectPath))
            {
                string absoluteProjectPath = Path.IsPathRooted(projectPath!)
                    ? projectPath!
                    : Path.Combine(workingDirectoryPath, projectPath!);

                if (File.Exists(absoluteProjectPath))
                {
                    string? projectDir = Path.GetDirectoryName(absoluteProjectPath);
                    if (!string.IsNullOrEmpty(projectDir))
                    {
                        return projectDir;
                    }
                }
                else if (Directory.Exists(absoluteProjectPath))
                {
                    return absoluteProjectPath;
                }
            }

            return workingDirectoryPath;
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

