using MultiPublish.Publish;

namespace MultiPublish.Abstractions
{
    public interface IPublishDirectoryFinder
    {
        string? FindPublishDirectory(PublishConfiguration configuration, IReadOnlyList<string> publishArgs, string workingDirectoryPath);
    }
}

