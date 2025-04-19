using NuVerNet.DependencyResolver.SolutionReader;

namespace NuVerNet.DependencyResolver;

public class CsprojDependencyResolver
{
    protected CsprojDependencyResolver()
    {
    }

    public static CsprojDependencyResolver New() => new();

    public async Task<ProjectModel> GetDependentProjectsAsync(string csprojPath, string solutionPath)
    {
        var projectModel = GetProjectModel(csprojPath);

        var allCsprojModels = await GetCsprojContentsOfSolutionAsync(solutionPath);

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        FindDependentProjectsRecursively(projectModel, allCsprojModels, visited);

        return projectModel;
    }

    private void FindDependentProjectsRecursively(
        ProjectModel projectModel, CsprojModel[] allCsprojModels, HashSet<string> visited
    )
    {
        if (visited.Contains(projectModel.Path)) return;

        visited.Add(projectModel.Path);

        foreach (var csprojModel in allCsprojModels)
        {
            var csprojReader = GetCsprojReader(csprojModel.Path);

            if (csprojReader.HasProjectReference(projectModel.Name))
            {
                var usedInProjectModel = new ProjectModel
                {
                    Name = csprojReader.GetProjectName(),
                    Path = csprojModel.Path,
                    CsprojContent = csprojReader.GetContent(),
                    Version = csprojReader.GetProjectVersion()
                };

                projectModel.UsedIn.Add(usedInProjectModel);

                FindDependentProjectsRecursively(usedInProjectModel, allCsprojModels, visited);
            }
        }
    }

    protected virtual CsprojReader.CsprojReader GetCsprojReader(string csprojPath)
    {
        return CsprojReader.CsprojReader.New().WithCsprojPath(csprojPath);
    }

    protected virtual ProjectModel GetProjectModel(string csprojPath)
    {
        var csprojReader = CsprojReader.CsprojReader.New().WithCsprojPath(csprojPath);
        csprojReader.Load();

        return new ProjectModel
        {
            Name = csprojReader.GetProjectName(),
            Path = csprojPath,
            CsprojContent = csprojReader.GetContent(),
            Version = csprojReader.GetProjectVersion(),
        };
    }

    protected virtual async Task<CsprojModel[]> GetCsprojContentsOfSolutionAsync(string solutionPath)
    {
        return await SolutionReader.SolutionReader.New().WithSolutionPath(solutionPath).GetCsprojsAsync();
    }
}