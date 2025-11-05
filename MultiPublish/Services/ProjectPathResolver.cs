using MultiPublish.Abstractions;

namespace MultiPublish.Services
{
    public class ProjectPathResolver : IProjectPathResolver
    {
        public string? ResolveProjectPath(string[] originalArgs)
        {
            for (int i = 0; i < originalArgs.Length; i += 1)
            {
                string token = originalArgs[i];
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }
                if (token.StartsWith("-"))
                {
                    continue;
                }
                if (token.StartsWith("["))
                {
                    continue;
                }
                if (this.LooksLikeProjectOrSolution(token))
                {
                    return token;
                }
            }

            return null;
        }

        private bool LooksLikeProjectOrSolution(string token)
        {
            if (token.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (token.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}

