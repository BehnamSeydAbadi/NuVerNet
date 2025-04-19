using NuVerNet.DependencyResolver;

namespace NuVerNet.Test.Orchestrator.Stubs;

public class StubOrchestrator : NuVerNet.Orchestrator
{
    private ProjectModel _projectModel;

    public new static StubOrchestrator New() => new();

    public StubOrchestrator WithProjectModel(ProjectModel projectModel)
    {
        _projectModel = projectModel;
        return this;
    }

    protected override async Task<ProjectModel> GetProjectModelAsync()
    {
        await Task.CompletedTask;
        return _projectModel;
    }
}