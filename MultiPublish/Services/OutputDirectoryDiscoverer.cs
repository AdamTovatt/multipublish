using MultiPublish.Abstractions;

namespace MultiPublish.Services
{
    public class OutputDirectoryDiscoverer : IOutputDirectoryDiscoverer
    {
        public string? TryGetOutputDirectoryFromArgs(IReadOnlyList<string> publishArgs)
        {
            for (int i = 0; i < publishArgs.Count; i += 1)
            {
                string current = publishArgs[i];
                if (current == "-o" || current == "--output")
                {
                    if (i + 1 < publishArgs.Count)
                    {
                        return publishArgs[i + 1];
                    }
                }
            }

            return null;
        }
    }
}

