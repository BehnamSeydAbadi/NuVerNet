using NuVerNet.DependencyResolver.SolutionReader;

namespace NuVerNet.DependencyResolver;

public class CsprojDependencyResolver
{
    protected CsprojDependencyResolver()
    {
    }

    public static CsprojDependencyResolver New() => new();

    public ProjectModel GetDependentProjects(string csprojAbsolutePath, string solutionPath)
    {
        var projectModel = GetProjectModel(csprojAbsolutePath);

        var allCsprojModels = GetCsprojContentsOfSolution(solutionPath);

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
            var csprojReader = GetCsprojReader(csprojModel.AbsolutePath);

            var usedInProjectModel = csprojReader.GetProjectModel();

            if (csprojReader.HasProjectReference(projectModel.Name) && visited.Contains(usedInProjectModel.Path) is false)
            {
                projectModel.UsedIn.Add(usedInProjectModel);

                FindDependentProjectsRecursively(usedInProjectModel, allCsprojModels, visited);
            }
        }
    }

    protected virtual CsprojReader.CsprojReader GetCsprojReader(string csprojPath)
    {
        var csprojReader = CsprojReader.CsprojReader.New().WithCsprojPath(csprojPath);
        csprojReader.Load();
        return csprojReader;
    }

    protected virtual ProjectModel GetProjectModel(string csprojPath)
    {
        var csprojReader = CsprojReader.CsprojReader.New().WithCsprojPath(csprojPath);
        csprojReader.Load();

        return new ProjectModel
        {
            Name = csprojReader.GetProjectName(),
            Path = csprojPath,
            AbsolutePath = csprojPath,
            CsprojContent = csprojReader.GetContent(),
            Version = csprojReader.GetProjectVersion(),
        };
    }

    protected virtual CsprojModel[] GetCsprojContentsOfSolution(string solutionPath)
    {
        var solutionReader = SolutionReader.SolutionReader.New().WithSolutionPath(solutionPath);
        solutionReader.Load();
        return solutionReader.GetCsprojs();
    }
}