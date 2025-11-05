namespace MultiPublish.Publishing
{
    public class DotnetPublishResult
    {
        public DotnetPublishResult(bool succeeded, int exitCode, string standardOutput, string standardError)
        {
            this.Succeeded = succeeded;
            this.ExitCode = exitCode;
            this.StandardOutput = standardOutput;
            this.StandardError = standardError;
        }

        public bool Succeeded { get; }

        public int ExitCode { get; }

        public string StandardOutput { get; }

        public string StandardError { get; }
    }
}

