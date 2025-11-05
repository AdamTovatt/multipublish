using MultiPublish.ArgParsing;

namespace MultiPublish.Tests
{
    public class ArgumentParserTests
    {
        [Fact]
        public void Parse_SingleRuntime_ParsesOneRuntime()
        {
            string[] args = new string[] { "-r", "win-x64" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.Single(result.Runtimes);
            Assert.Contains("win-x64", result.Runtimes);
        }

        [Fact]
        public void Parse_ArrayRuntimes_ParsesMultipleRuntimes()
        {
            string[] args = new string[] { "--runtime", "[win-x64, win-x86, osx-arm64]" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.Equal(3, result.Runtimes.Count);
            Assert.Contains("win-x64", result.Runtimes);
            Assert.Contains("win-x86", result.Runtimes);
            Assert.Contains("osx-arm64", result.Runtimes);
        }

        [Fact]
        public void Parse_SelfContainedArray_ParsesTrueAndFalse()
        {
            string[] args = new string[] { "--self-contained", "[true, false]" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.Equal(2, result.SelfContainedOptions.Count);
            Assert.Contains(true, result.SelfContainedOptions);
            Assert.Contains(false, result.SelfContainedOptions);
        }

        [Fact]
        public void Parse_DefaultZipEnabled_IsTrue()
        {
            string[] args = Array.Empty<string>();

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.True(result.ZipEnabled);
        }

        [Fact]
        public void Parse_NoZipFlag_DisablesZip()
        {
            string[] args = new string[] { "--no-zip" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.False(result.ZipEnabled);
        }

        [Fact]
        public void Parse_PassThroughArgs_ArePreserved()
        {
            string[] args = new string[] { "-c", "Release", "-f", "net8.0", "/p:PublishSingleFile=true", "--nologo" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.Equal(6, result.PassThroughArgs.Count);
            Assert.Equal("-c", result.PassThroughArgs[0]);
            Assert.Equal("Release", result.PassThroughArgs[1]);
            Assert.Equal("-f", result.PassThroughArgs[2]);
            Assert.Equal("net8.0", result.PassThroughArgs[3]);
            Assert.Contains("/p:PublishSingleFile=true", result.PassThroughArgs);
            Assert.Contains("--nologo", result.PassThroughArgs);
        }

        [Fact]
        public void Parse_ArrayRuntimes_SplitTokens_ParsesAll()
        {
            string[] args = new string[] { "-r", "[win-x64,", "win-x86,", "linux-arm64]" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.Equal(3, result.Runtimes.Count);
            Assert.Contains("win-x64", result.Runtimes);
            Assert.Contains("win-x86", result.Runtimes);
            Assert.Contains("linux-arm64", result.Runtimes);
        }

        [Fact]
        public void Parse_SelfContained_SplitTokens_ParsesBoth()
        {
            string[] args = new string[] { "--self-contained", "[true,", "false]" };

            ParsedArguments result = ArgumentParser.Parse(args);

            Assert.Equal(2, result.SelfContainedOptions.Count);
            Assert.Contains(true, result.SelfContainedOptions);
            Assert.Contains(false, result.SelfContainedOptions);
        }
    }
}

