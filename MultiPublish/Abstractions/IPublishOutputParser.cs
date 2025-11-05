using MultiPublish.Publishing;

namespace MultiPublish.Abstractions
{
    public interface IPublishOutputParser
    {
        string? ExtractPublishDirectory(DotnetPublishResult publishResult);
    }
}

