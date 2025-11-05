namespace MultiPublish.Abstractions
{
    public interface IOutputDirectoryDiscoverer
    {
        string? TryGetOutputDirectoryFromArgs(IReadOnlyList<string> publishArgs);
    }
}

