using MultiPublish.ArgParsing;

namespace MultiPublish.Abstractions
{
    public interface IPublishOrchestrator
    {
        void Run(ParsedArguments parsed, string[] originalArgs);
    }
}

