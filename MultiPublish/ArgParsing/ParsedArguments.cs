namespace MultiPublish.ArgParsing
{
    public class ParsedArguments
    {
        public ParsedArguments(
            IReadOnlyList<string> runtimes,
            IReadOnlyList<bool> selfContainedOptions,
            IReadOnlyList<string> passThroughArgs,
            bool zipEnabled)
        {
            this.Runtimes = runtimes;
            this.SelfContainedOptions = selfContainedOptions;
            this.PassThroughArgs = passThroughArgs;
            this.ZipEnabled = zipEnabled;
        }

        public IReadOnlyList<string> Runtimes { get; }

        public IReadOnlyList<bool> SelfContainedOptions { get; }

        public IReadOnlyList<string> PassThroughArgs { get; }

        public bool ZipEnabled { get; }
    }
}

