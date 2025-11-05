using MultiPublish.ArgParsing;
using MultiPublish.Publish;

namespace MultiPublish
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParsedArguments parsed = ArgumentParser.Parse(args);

            // Extract possible project/solution path if a plausible path token is present
            string? projectPath = GetFirstLikelyProjectPath(args);
            string projectName = GetProjectName(projectPath);

            IReadOnlyList<PublishConfiguration> configurations = PublishCommandGenerator.GenerateConfigurations(parsed);
            DotnetPublishExecutor executor = new DotnetPublishExecutor(new DotnetCliRunner());

            foreach (PublishConfiguration configuration in configurations)
            {
                List<string> publishArgs = new List<string>();
                if (!string.IsNullOrEmpty(projectPath))
                {
                    publishArgs.Add(projectPath!);
                }

                IReadOnlyList<string> baseArgs = BuildBaseArgsExcludingSpecial(parsed.PassThroughArgs);
                foreach (string a in PublishCommandGenerator.BuildCommandArgs(baseArgs, configuration))
                {
                    publishArgs.Add(a);
                }

                DotnetPublishResult result = executor.Execute(publishArgs);
                Console.WriteLine(result.StandardOutput);
                if (!result.Succeeded)
                {
                    Console.Error.WriteLine(result.StandardError);
                    continue;
                }

                if (!parsed.ZipEnabled)
                {
                    continue;
                }

                string? outputDir = TryGetOutputDirectoryFromArgs(publishArgs) ?? FindPublishDirectory(configuration, publishArgs);

                if (!string.IsNullOrEmpty(outputDir) && Directory.Exists(outputDir))
                {
                    string binDir = GetBinDirectoryFromPublish(outputDir!);
                    string zipPath = PublishOutputZipper.CreateZip(projectName, binDir, outputDir!, configuration);
                    Console.WriteLine($"Created zip: {zipPath}");
                }
            }
        }

        private static string? GetFirstLikelyProjectPath(IReadOnlyList<string> args)
        {
            foreach (string a in args)
            {
                if (a.StartsWith("-"))
                {
                    continue;
                }
                if (a.StartsWith("["))
                {
                    continue;
                }
                if (LooksLikeProjectOrDirectory(a))
                {
                    return a;
                }
            }

            return null;
        }

        private static bool LooksLikeProjectOrDirectory(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }
            if (token.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || token.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            // Only accept explicit project/solution files; do not accept bare directories
            return false;
        }

        private static string GetProjectName(string? projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                return new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
            }

            if (File.Exists(projectPath))
            {
                return Path.GetFileNameWithoutExtension(projectPath);
            }

            if (Directory.Exists(projectPath))
            {
                return new DirectoryInfo(projectPath).Name;
            }

            return new DirectoryInfo(Directory.GetCurrentDirectory()).Name;
        }

        private static IReadOnlyList<string> BuildBaseArgsExcludingSpecial(IReadOnlyList<string> args)
        {
            // Our parser already omits special tokens it consumed. Just return as-is.
            List<string> result = new List<string>();
            foreach (string a in args)
            {
                result.Add(a);
            }

            return result;
        }

        private static string? TryGetOutputDirectoryFromArgs(IReadOnlyList<string> args)
        {
            for (int i = 0; i < args.Count; i += 1)
            {
                string current = args[i];
                if (current == "-o" || current == "--output")
                {
                    if (i + 1 < args.Count)
                    {
                        return args[i + 1];
                    }
                }
            }

            return null;
        }

        private static string? FindPublishDirectory(PublishConfiguration configuration, IReadOnlyList<string> publishArgs)
        {
            string configurationValue = GetOptionValue(publishArgs, "-c", "--configuration") ?? "Debug";
            string? frameworkValue = GetOptionValue(publishArgs, "-f", "--framework");

            string binRoot = Path.Combine(Directory.GetCurrentDirectory(), "bin");
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

        private static string? GetOptionValue(IReadOnlyList<string> args, string shortName, string longName)
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

        private static string GetBinDirectoryFromPublish(string publishDirectoryPath)
        {
            string? dir = Path.GetDirectoryName(publishDirectoryPath);
            while (!string.IsNullOrEmpty(dir))
            {
                string name = Path.GetFileName(dir);
                if (string.Equals(name, "bin", StringComparison.OrdinalIgnoreCase))
                {
                    return dir;
                }

                dir = Path.GetDirectoryName(dir);
            }

            return Path.Combine(Directory.GetCurrentDirectory(), "bin");
        }
    }
}
