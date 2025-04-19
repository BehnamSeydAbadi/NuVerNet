using NuVerNet.DependencyResolver;
using NuVerNet.TreeTraversal;

namespace NuVerNet;

public class Orchestrator
{
    private string _csprojPath;
    private string _solutionPath;
    private ProjectModel _projectModel;
    private Tree<ProjectModel> _tree;

    protected Orchestrator()
    {
    }

    public static Orchestrator New() => new();

    public Orchestrator WithCsprojPath(string csprojPath)
    {
        _csprojPath = csprojPath;
        return this;
    }

    public Orchestrator WithSolutionPath(string solutionPath)
    {
        _solutionPath = solutionPath;
        return this;
    }

    public async Task LoadAsync()
    {
        _projectModel = await GetProjectModelAsync();
        _tree = Tree<ProjectModel>.New().WithRootNode(ToNode(_projectModel));
    }

    public void BumpVersions()
    {
        _tree.TraverseDfs(pm => pm.CsprojContent.BumpVersion());
    }

    public void PackProjectsIntoNugets()
    {
        throw new NotImplementedException();
    }

    public void PushProjectsToNugetServer()
    {
        throw new NotImplementedException();
    }


    protected virtual async Task<ProjectModel> GetProjectModelAsync()
    {
        return await CsprojDependencyResolver.New().GetDependentProjectsAsync(_csprojPath, _solutionPath);
    }

    private Node<ProjectModel> ToNode(ProjectModel projectModel)
    {
        var node = new Node<ProjectModel>(projectModel);

        foreach (var usedInProjectModel in projectModel.UsedIn)
        {
            node.AddChild(usedInProjectModel);

            ToNode(usedInProjectModel);
        }

        return node;
    }
}