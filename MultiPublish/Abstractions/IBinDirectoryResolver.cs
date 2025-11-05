namespace MultiPublish.Abstractions
{
    public interface IBinDirectoryResolver
    {
        string ResolveBinDirectoryFromPublish(string publishDirectoryPath, string fallbackRootPath);
    }
}

