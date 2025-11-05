namespace MultiPublish.Publishing
{
    public interface IDotnetProcessRunner
    {
        int Run(IReadOnlyList<string> arguments, out string standardOutput, out string standardError);
    }
}

