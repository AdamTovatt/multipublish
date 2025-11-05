using System.Diagnostics;

namespace MultiPublish.Publishing
{
    public class DotnetCliRunner : IDotnetProcessRunner
    {
        public int Run(IReadOnlyList<string> arguments, out string standardOutput, out string standardError)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("dotnet")
            {
                ArgumentList = { },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            foreach (string arg in arguments)
            {
                startInfo.ArgumentList.Add(arg);
            }

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                standardOutput = output;
                standardError = error;
                return process.ExitCode;
            }
        }
    }
}

