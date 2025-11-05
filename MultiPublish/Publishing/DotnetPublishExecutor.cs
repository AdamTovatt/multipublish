using System.Collections.Generic;

namespace MultiPublish.Publishing
{
    public class DotnetPublishExecutor
    {
        private readonly IDotnetProcessRunner dotnetProcessRunner;

        public DotnetPublishExecutor(IDotnetProcessRunner dotnetProcessRunner)
        {
            this.dotnetProcessRunner = dotnetProcessRunner;
        }

        public DotnetPublishResult Execute(IReadOnlyList<string> publishArgs)
        {
            List<string> fullArgs = new List<string>();
            fullArgs.Add("publish");
            foreach (string a in publishArgs)
            {
                fullArgs.Add(a);
            }

            string stdout;
            string stderr;
            int exitCode = this.dotnetProcessRunner.Run(fullArgs, out stdout, out stderr);

            return new DotnetPublishResult(exitCode == 0, exitCode, stdout, stderr);
        }
    }
}
