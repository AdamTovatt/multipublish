namespace MultiPublish.Abstractions
{
    public interface IProjectPathResolver
    {
        string? ResolveProjectPath(string[] originalArgs);
    }
}

