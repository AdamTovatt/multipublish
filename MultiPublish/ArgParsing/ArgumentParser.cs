namespace MultiPublish.ArgParsing
{
    public static class ArgumentParser
    {
        public static ParsedArguments Parse(string[] args)
        {
            List<string> runtimes = new List<string>();
            List<bool> selfContainedOptions = new List<bool>();
            List<string> passThroughArgs = new List<string>();
            bool zipEnabled = true;

            int index = 0;
            while (index < args.Length)
            {
                string current = args[index];

                if (current == "--no-zip")
                {
                    zipEnabled = false;
                    index += 1;
                    continue;
                }

                if (current == "-r" || current == "--runtime" || current.StartsWith("-r[") || current.StartsWith("--runtime["))
                {
                    // Support forms: -r [a, b], --runtime [a, b], -r[a,b], --runtime[a,b]
                    string? value = null;
                    if (current.StartsWith("-r[") || current.StartsWith("--runtime["))
                    {
                        value = ExtractBracketContentFromSameToken(current);
                        index += 1;
                    }
                    else if (index + 1 < args.Length)
                    {
                        value = ConsumeBracketedOrSingleValue(args, ref index);
                        // ConsumeBracketedOrSingleValue advances index to the last consumed token; move past it
                        index += 1;
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        foreach (string runtime in ParseMaybeArray(value!))
                        {
                            if (!string.IsNullOrWhiteSpace(runtime))
                            {
                                runtimes.Add(runtime);
                            }
                        }
                    }

                    continue;
                }

                if (current == "--self-contained" || current == "--sc" || current.StartsWith("--self-contained[") || current.StartsWith("--sc["))
                {
                    // Handle as switch (true) or as value/array following
                    if (current.StartsWith("--self-contained[") || current.StartsWith("--sc["))
                    {
                        string embedded = ExtractBracketContentFromSameToken(current);
                        foreach (string token in ParseMaybeArray(embedded))
                        {
                            bool parsed;
                            if (bool.TryParse(token, out parsed))
                            {
                                selfContainedOptions.Add(parsed);
                            }
                        }

                        index += 1;
                    }
                    else if (index + 1 < args.Length && !IsNextOption(args[index + 1]))
                    {
                        string valueToken = ConsumeBracketedOrSingleValue(args, ref index);
                        foreach (string token in ParseMaybeArray(valueToken))
                        {
                            bool parsed;
                            if (bool.TryParse(token, out parsed))
                            {
                                selfContainedOptions.Add(parsed);
                            }
                        }
                        // Move past the consumed value
                        index += 1;
                    }
                    else
                    {
                        selfContainedOptions.Add(true);
                        index += 1;
                    }

                    continue;
                }

                if (current == "--no-self-contained")
                {
                    selfContainedOptions.Add(false);
                    index += 1;
                    continue;
                }

                // Unrecognized or pass-through; keep as-is
                passThroughArgs.Add(current);
                index += 1;
            }

            return new ParsedArguments(runtimes, selfContainedOptions, passThroughArgs, zipEnabled);
        }

        private static bool IsNextOption(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            return token.StartsWith("-");
        }

        private static IReadOnlyList<string> ParseMaybeArray(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            string trimmed = value.Trim();
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                string inner = trimmed.Substring(1, trimmed.Length - 2);
                string[] parts = inner.Split(',');
                List<string> results = new List<string>();
                foreach (string part in parts)
                {
                    string item = part.Trim();
                    if (item.Length > 0)
                    {
                        results.Add(item);
                    }
                }

                return results;
            }

            return new string[] { trimmed };
        }

        private static string ExtractBracketContentFromSameToken(string token)
        {
            int start = token.IndexOf('[');
            int end = token.LastIndexOf(']');
            if (start >= 0 && end > start)
            {
                return token.Substring(start, end - start + 1);
            }

            return token;
        }

        private static string ConsumeBracketedOrSingleValue(string[] args, ref int index)
        {
            // index points to the option token; value is at index+1
            int valueIndex = index + 1;
            if (valueIndex >= args.Length)
            {
                return string.Empty;
            }

            string first = args[valueIndex];
            if (!string.IsNullOrEmpty(first) && first.StartsWith("["))
            {
                List<string> collected = new List<string>();
                collected.Add(first);
                int j = valueIndex;
                while (j + 1 < args.Length && !args[j].EndsWith("]"))
                {
                    j += 1;
                    collected.Add(args[j]);
                    if (args[j].EndsWith("]"))
                    {
                        break;
                    }
                }

                // Advance caller index to the last consumed value token
                index = j;
                string joined = string.Join(" ", collected);
                return joined;
            }

            // Single value; advance index to value token
            index = valueIndex;
            return first;
        }
    }
}

