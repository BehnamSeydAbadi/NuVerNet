using System.Xml.Linq;
using NuVerNet.DependencyResolver.Extensions;
using NuVerNet.DependencyResolver.ViewModels;

namespace NuVerNet.DependencyResolver;

public class CsprojDependencyResolver
{
    public async Task<ProjectModel[]> GetDependentProjectsAsync(string csprojPath, string solutionPath = "")
    {
        var csprojContent = await GetCsprojContentAsync(csprojPath);

        var projectReferenceElements = XDocument.Load(new StringReader(csprojContent)).GetProjectReferenceElements();

        var csprojPathsOfSolution = GetCsprojContentsOfSolutionAsync(solutionPath);

        var projectModelsIndexedByProjectName = new Dictionary<string, ProjectModel>();

        foreach (var projectReferenceElement in projectReferenceElements)
        {
            var projectName = projectReferenceElement.GetProjectName();
            var relativeProjectPath = projectReferenceElement.GetRelativeProjectPath();

            projectModelsIndexedByProjectName.Add(
                key: projectName,
                new ProjectModel
                {
                    ProjectName = projectName,
                    ProjectPath = relativeProjectPath,
                    UsedIn = []
                }
            );
        }

        return projectModelsIndexedByProjectName.Values.ToArray();
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