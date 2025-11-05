namespace MultiPublish.Abstractions
{
    public interface IProjectNameResolver
    {
        string ResolveProjectName(string? projectPath, string currentDirectoryPath);
    }
}

