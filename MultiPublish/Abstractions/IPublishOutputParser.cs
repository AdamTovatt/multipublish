using MultiPublish.Publish;

namespace MultiPublish.Abstractions
{
    public interface IPublishOutputParser
    {
        string? ExtractPublishDirectory(DotnetPublishResult publishResult);
    }
}

