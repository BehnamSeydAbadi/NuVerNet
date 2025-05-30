using NuVerNet.DependencyResolver;
using NuVerNet.DependencyResolver.CsprojReader;
using NuVerNet.DependencyResolver.SolutionReader;

namespace NuVerNet.Test.DependencyResolver.Stubs;

public class StubCsprojDependencyResolver : CsprojDependencyResolver
{
    private ProjectModel _projectModel;
    private Csproj[] _csprojsOfSolution = [];
    private Dictionary<string, StubCsprojReader> _csprojReadersIndexedByCsprojPath = [];

    private StubCsprojDependencyResolver() : base()
    {
    }

    public new static StubCsprojDependencyResolver New() => new();

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
        _csprojReadersIndexedByCsprojPath = csprojReaders.ToDictionary(cp => cp.AbsoluteProjectPath);
        return this;
    }

    protected override ProjectModel GetProjectModel(string csprojPath) => _projectModel;

    protected override CsprojModel[] GetCsprojContentsOfSolution(string solutionPath)
    {
        return _csprojsOfSolution.Any() is false
            ? []
            : _csprojsOfSolution.Select(cm => new CsprojModel(cm.Path, cm.Content, solutionPath)).ToArray();
    }

    protected override CsprojReader GetCsprojReader(string csprojPath)
    {
        return _csprojReadersIndexedByCsprojPath[csprojPath];
    }

    public record Csproj(string Path, string Content);
}