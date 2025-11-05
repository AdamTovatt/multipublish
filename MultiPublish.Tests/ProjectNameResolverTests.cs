using MultiPublish.Abstractions;
using MultiPublish.Services;

namespace MultiPublish.Tests
{
    public class ProjectNameResolverTests
    {
        [Fact]
        public void ResolveProjectName_UsesCurrentDirectory_WhenNoPath()
        {
            IProjectNameResolver resolver = new ProjectNameResolver();
            string cwd = Path.Combine("C:", "Temp", "Sample");

            string name = resolver.ResolveProjectName(null, cwd);

            Assert.Equal("Sample", name);
        }

        [Fact]
        public void ResolveProjectName_FromCsprojPath()
        {
            IProjectNameResolver resolver = new ProjectNameResolver();
            string name = resolver.ResolveProjectName("MyProj.csproj", "C:");

            Assert.Equal("MyProj", name);
        }
    }
}

