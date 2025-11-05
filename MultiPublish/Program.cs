using MultiPublish.Abstractions;
using MultiPublish.ArgParsing;
using MultiPublish.Publish;
using MultiPublish.Services;

namespace MultiPublish
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParsedArguments parsed = ArgumentParser.Parse(args);

            IPublishOrchestrator orchestrator = new PublishOrchestrator(
                new ProjectPathResolver(),
                new ProjectNameResolver(),
                new OutputDirectoryDiscoverer(),
                new PublishDirectoryFinder(),
                new BinDirectoryResolver(),
                new DotnetPublishExecutor(new DotnetCliRunner())
            );

            orchestrator.Run(parsed, args);
        }
    }
}
