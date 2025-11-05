using MultiPublish.Abstractions;

namespace MultiPublish.Services
{
    public class ProjectNameResolver : IProjectNameResolver
    {
        public string ResolveProjectName(string? projectPath, string currentDirectoryPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                return new DirectoryInfo(currentDirectoryPath).Name;
            }

            if (projectPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
                projectPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileNameWithoutExtension(projectPath);
            }

            if (File.Exists(projectPath))
            {
                return Path.GetFileNameWithoutExtension(projectPath);
            }

            if (Directory.Exists(projectPath))
            {
                return new DirectoryInfo(projectPath).Name;
            }

            return new DirectoryInfo(currentDirectoryPath).Name;
        }
    }
}

