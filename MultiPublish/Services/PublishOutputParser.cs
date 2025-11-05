using System;
using MultiPublish.Abstractions;
using MultiPublish.Publishing;

namespace MultiPublish.Services
{
    public class PublishOutputParser : IPublishOutputParser
    {
        public string? ExtractPublishDirectory(DotnetPublishResult publishResult)
        {
            if (string.IsNullOrWhiteSpace(publishResult.StandardOutput))
            {
                return null;
            }

            string[] lines = publishResult.StandardOutput.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Contains("->"))
                {
                    if (trimmed.EndsWith("publish\\", StringComparison.OrdinalIgnoreCase) ||
                        trimmed.EndsWith("publish/", StringComparison.OrdinalIgnoreCase) ||
                        trimmed.EndsWith("publish", StringComparison.OrdinalIgnoreCase))
                    {
                        int arrowIndex = trimmed.IndexOf("->");
                        if (arrowIndex >= 0 && arrowIndex + 2 < trimmed.Length)
                        {
                            string pathPart = trimmed.Substring(arrowIndex + 2).Trim();
                            if (pathPart.EndsWith("\\", StringComparison.OrdinalIgnoreCase) || pathPart.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                            {
                                return pathPart.TrimEnd('\\', '/');
                            }

                            return pathPart;
                        }
                    }
                }
            }

            return null;
        }
    }
}

