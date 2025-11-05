namespace MultiPublish.Publishing
{
    public class PublishConfiguration
    {
        public PublishConfiguration(
            string? runtime,
            bool? selfContained)
        {
            this.Runtime = runtime;
            this.SelfContained = selfContained;
        }

        public string? Runtime { get; }

        public bool? SelfContained { get; }
    }
}

