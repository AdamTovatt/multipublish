using MultiPublish.Abstractions;

namespace MultiPublish.Services
{
    public class BinDirectoryResolver : IBinDirectoryResolver
    {
        public string ResolveBinDirectoryFromPublish(string publishDirectoryPath, string fallbackRootPath)
        {
            string? dir = Path.GetDirectoryName(publishDirectoryPath);
            while (!string.IsNullOrEmpty(dir))
            {
                string name = Path.GetFileName(dir);
                if (string.Equals(name, "bin", StringComparison.OrdinalIgnoreCase))
                {
                    return dir;
                }

                dir = Path.GetDirectoryName(dir);
            }

            return Path.Combine(fallbackRootPath, "bin");
        }
    }
}

