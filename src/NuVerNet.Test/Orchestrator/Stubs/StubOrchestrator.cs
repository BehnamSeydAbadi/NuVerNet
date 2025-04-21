using NuVerNet.DependencyResolver;

namespace NuVerNet.Test.Orchestrator.Stubs;

public class StubOrchestrator : NuVerNet.Orchestrator
{
    private ProjectModel _projectModel;
    public string CsprojAbsolutePath { get; private set; }

    public new static StubOrchestrator New() => new();

    public new StubOrchestrator WithSolutionPath(string solutionPath)
    {
        SolutionPath = solutionPath;
        return this;
    }

    public StubOrchestrator WithProjectModel(ProjectModel projectModel)
    {
        _projectModel = projectModel;
        return this;
    }

    protected override ProjectModel GetProjectModel() => _projectModel;

    protected override void WriteCsprojContent(string csprojPath, string csprojContent)
    {
        CsprojAbsolutePath = csprojPath;
    }
}