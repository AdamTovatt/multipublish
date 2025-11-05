using MultiPublish.Abstractions;
using MultiPublish.ArgParsing;
using MultiPublish.Publish;

namespace MultiPublish.Services
{
    public class PublishOrchestrator : IPublishOrchestrator
    {
        private readonly IProjectPathResolver projectPathResolver;
        private readonly IProjectNameResolver projectNameResolver;
        private readonly IOutputDirectoryDiscoverer outputDirectoryDiscoverer;
        private readonly IPublishDirectoryFinder publishDirectoryFinder;
        private readonly IBinDirectoryResolver binDirectoryResolver;
        private readonly IPublishOutputParser publishOutputParser;
        private readonly DotnetPublishExecutor executor;

        public PublishOrchestrator(
            IProjectPathResolver projectPathResolver,
            IProjectNameResolver projectNameResolver,
            IOutputDirectoryDiscoverer outputDirectoryDiscoverer,
            IPublishDirectoryFinder publishDirectoryFinder,
            IBinDirectoryResolver binDirectoryResolver,
            IPublishOutputParser publishOutputParser,
            DotnetPublishExecutor executor)
        {
            this.projectPathResolver = projectPathResolver;
            this.projectNameResolver = projectNameResolver;
            this.outputDirectoryDiscoverer = outputDirectoryDiscoverer;
            this.publishDirectoryFinder = publishDirectoryFinder;
            this.binDirectoryResolver = binDirectoryResolver;
            this.publishOutputParser = publishOutputParser;
            this.executor = executor;
        }

        public void Run(ParsedArguments parsed, string[] originalArgs)
        {
            string? projectPath = this.projectPathResolver.ResolveProjectPath(originalArgs);
            string projectName = this.projectNameResolver.ResolveProjectName(projectPath, Directory.GetCurrentDirectory());

            IReadOnlyList<PublishConfiguration> configurations = PublishCommandGenerator.GenerateConfigurations(parsed);

            foreach (PublishConfiguration configuration in configurations)
            {
                List<string> publishArgs = new List<string>();
                if (!string.IsNullOrEmpty(projectPath))
                {
                    publishArgs.Add(projectPath!);
                }

                IReadOnlyList<string> baseArgs = this.BuildBaseArgs(parsed.PassThroughArgs);
                foreach (string a in PublishCommandGenerator.BuildCommandArgs(baseArgs, configuration))
                {
                    publishArgs.Add(a);
                }

                DotnetPublishResult result = this.executor.Execute(publishArgs);
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

                string? outputDir = this.outputDirectoryDiscoverer.TryGetOutputDirectoryFromArgs(publishArgs);
                if (string.IsNullOrEmpty(outputDir))
                {
                    outputDir = this.publishOutputParser.ExtractPublishDirectory(result);
                }

                if (string.IsNullOrEmpty(outputDir))
                {
                    outputDir = this.publishDirectoryFinder.FindPublishDirectory(configuration, publishArgs, Directory.GetCurrentDirectory());
                }

                if (!string.IsNullOrEmpty(outputDir) && Directory.Exists(outputDir))
                {
                    string binDir = this.binDirectoryResolver.ResolveBinDirectoryFromPublish(outputDir!, Directory.GetCurrentDirectory());
                    string zipPath = PublishOutputZipper.CreateZip(projectName, binDir, outputDir!, configuration);
                    Console.WriteLine("Created zip: " + zipPath);
                }
                else
                {
                    Console.Error.WriteLine("Could not locate publish directory for runtime: " + configuration.Runtime);
                }
            }
        }

        private IReadOnlyList<string> BuildBaseArgs(IReadOnlyList<string> passThroughArgs)
        {
            List<string> result = new List<string>();
            foreach (string a in passThroughArgs)
            {
                result.Add(a);
            }

            return result;
        }
    }
}

