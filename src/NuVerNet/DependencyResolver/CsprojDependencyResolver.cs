using System.Xml.Linq;
using NuVerNet.DependencyResolver.Extensions;
using NuVerNet.DependencyResolver.ViewModels;

namespace NuVerNet.DependencyResolver;

public class CsprojDependencyResolver
{
    public async Task<ProjectModel> GetDependentProjectsAsync(string csprojPath, string solutionPath = "")
    {
        var projectModel = await GetProjectModelAsync(csprojPath);

        // var projectReferenceElements = xDocument.GetProjectReferenceElements();
        //
        // var usedInProjectModels = new List<ProjectModel>();
        //
        // var csprojPathsOfSolution = GetCsprojContentsOfSolutionAsync(solutionPath);
        //
        // foreach (var projectReferenceElement in projectReferenceElements)
        // {
        //     var projectName = projectReferenceElement.GetProjectName();
        //     var relativeProjectPath = projectReferenceElement.GetRelativeProjectPath();
        //
        //     usedInProjectModels.Add(
        //         key: projectName,
        //         new ProjectModel
        //         {
        //             Name = projectName,
        //             Path = relativeProjectPath,
        //             UsedIn = []
        //         }
        //     );
        // }

        return projectModel;
    }

    private async Task<ProjectModel> GetProjectModelAsync(string csprojPath)
    {
        var csprojReader = CsprojReader.New().WithCsprojPath(csprojPath);
        await csprojReader.LoadAsync();

        return new ProjectModel
        {
            Name = csprojReader.GetProjectName(),
            Path = csprojPath,
            Version = csprojReader.GetProjectVersion(),
        };
    }


    protected virtual async Task<string> GetCsprojContentAsync(string csprojPath) => await File.ReadAllTextAsync(csprojPath);

    protected virtual async Task<string[]> GetCsprojContentsOfSolutionAsync(string solutionPath)
    {
        var csprojPaths = Directory.GetFiles(solutionPath, "*.csproj", new EnumerationOptions
        {
            RecurseSubdirectories = true
        });

        return await Task.WhenAll(
            csprojPaths.Select(p => File.ReadAllTextAsync(p))
        );
    }
}