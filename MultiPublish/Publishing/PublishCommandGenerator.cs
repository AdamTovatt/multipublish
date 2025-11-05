using MultiPublish.ArgParsing;

namespace MultiPublish.Publishing
{
    public static class PublishCommandGenerator
    {
        public static IReadOnlyList<PublishConfiguration> GenerateConfigurations(ParsedArguments parsed)
        {
            List<PublishConfiguration> configurations = new List<PublishConfiguration>();

            IReadOnlyList<string> runtimes = parsed.Runtimes.Count == 0
                ? new List<string?> { null } as IReadOnlyList<string>
                : parsed.Runtimes;

            IReadOnlyList<bool?> selfContainedList;
            if (parsed.SelfContainedOptions.Count == 0)
            {
                selfContainedList = new List<bool?> { null };
            }
            else
            {
                List<bool?> temp = new List<bool?>();
                foreach (bool value in parsed.SelfContainedOptions)
                {
                    temp.Add(value);
                }

                selfContainedList = temp;
            }

            foreach (string? runtime in ToNullableStringList(runtimes))
            {
                foreach (bool? selfContained in selfContainedList)
                {
                    configurations.Add(new PublishConfiguration(runtime, selfContained));
                }
            }

            return configurations;
        }

        public static IReadOnlyList<string> BuildCommandArgs(IReadOnlyList<string> baseArgs, PublishConfiguration configuration)
        {
            List<string> args = new List<string>();
            foreach (string a in baseArgs)
            {
                args.Add(a);
            }

            if (!string.IsNullOrEmpty(configuration.Runtime))
            {
                args.Add("-r");
                args.Add(configuration.Runtime!);
            }

            if (configuration.SelfContained.HasValue)
            {
                if (configuration.SelfContained.Value)
                {
                    args.Add("--self-contained");
                }
                else
                {
                    args.Add("--no-self-contained");
                }
            }

            return args;
        }

        private static IReadOnlyList<string?> ToNullableStringList(IReadOnlyList<string> list)
        {
            List<string?> results = new List<string?>();
            foreach (string item in list)
            {
                results.Add(item);
            }

            return results;
        }
    }
}

