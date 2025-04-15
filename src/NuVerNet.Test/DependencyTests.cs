using System.Xml.Linq;
using FluentAssertions;

namespace NuVerNet.Test;

public class DependencyTests
{
    [Fact(DisplayName =
        "There is a .NET projects, When dependency logic works, Then null should be returned successfully")]
    public async Task DependencyLogicWorksOnOneProjectSuccessfully()
    {
        var dependency = new Dependency();

        var dependentProjects = await dependency.GetDependentProjectsPathAsync(
            projectPath:
            @"C:\Users\babakm\source\repos\WinServiceBridge\WinServiceBridge.WorkerService\WinServiceBridge.WorkerService.csproj"
        );

        dependentProjects.Should().BeEmpty();
    }

    [Fact(DisplayName =
        "There are two .NET projects which one of them is dependent to another one, When dependency logic works, Then the dependent project should be returned successfully")]
    public async Task DependencyLogicWorksOnTwoDependentProjectsSuccessfully()
    {
        var dependency = new Dependency();

        var dependentProjects = await dependency.GetDependentProjectsPathAsync(
            projectPath:
            @"C:\Users\babakm\source\repos\NuVerNet\src\NuVerNet.Test\NuVerNet.Test.csproj"
        );

        dependentProjects.Should().NotBeEmpty();
    }
}

public class Dependency
{
    public async Task<string[]> GetDependentProjectsPathAsync(string projectPath)
    {
        var csprojStreamReader = File.OpenText(projectPath);

        var doc = await XDocument.LoadAsync(csprojStreamReader, LoadOptions.None, CancellationToken.None);

        var projectReferenceElements = doc.Descendants("ProjectReference");

        var dependentProjects = new List<string>();

        foreach (var projectReferenceElement in projectReferenceElements)
        {
            var erer = projectReferenceElement.Attribute(XName.Get("Include"))!.Value;
            dependentProjects.Add(erer);
        }

        return dependentProjects.ToArray();
    }
}