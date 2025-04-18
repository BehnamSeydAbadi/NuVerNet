using NuVerNet.DependencyResolver;
using NuVerNet.DependencyResolver.CsprojReader;
using NuVerNet.DependencyResolver.SolutionReader;
using NuVerNet.DependencyResolver.ViewModels;

namespace NuVerNet.Test.DependencyResolver.Stubs;

public class StubCsprojDependencyResolver : CsprojDependencyResolver
{
    private ProjectModel _projectModel;
    private Csproj[] _csprojsOfSolution = [];
    private Dictionary<string, StubCsprojReader> _csprojReadersIndexedByCsprojPath = [];

    private StubCsprojDependencyResolver()
    {
    }

    public static StubCsprojDependencyResolver New() => new();

    public StubCsprojDependencyResolver WithProjectModel(ProjectModel projectModel)
    {
        _projectModel = projectModel;
        return this;
    }

    public StubCsprojDependencyResolver WithCsprojContentsOfSolution(params Csproj[] csprojs)
    {
        _csprojsOfSolution = csprojs;
        return this;
    }

    public StubCsprojDependencyResolver WithCsprojReaders(params StubCsprojReader[] csprojReaders)
    {
        _csprojReadersIndexedByCsprojPath = csprojReaders.ToDictionary(cp => cp.ProjectPath);
        return this;
    }

    protected override ProjectModel GetProjectModel(string csprojPath) => _projectModel;

    protected override async Task<CsprojModel[]> GetCsprojContentsOfSolutionAsync(string solutionPath)
    {
        await Task.CompletedTask;
        return _csprojsOfSolution.Any() is false
            ? []
            : _csprojsOfSolution.Select(cm => new CsprojModel(cm.Path, cm.Content)).ToArray();
    }

    protected override CsprojReader GetCsprojReader(string csprojPath)
    {
        return _csprojReadersIndexedByCsprojPath[csprojPath];
    }

    public record Csproj(string Path, string Content);
}