using NuVerNet.DependencyResolver;
using NuVerNet.TreeTraversal;

namespace NuVerNet;

public class Orchestrator
{
    private string _csprojPath;
    protected string SolutionPath;
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
        SolutionPath = solutionPath;
        return this;
    }

    public async Task LoadAsync()
    {
        _projectModel = await GetProjectModelAsync();
        _tree = Tree<ProjectModel>.New().WithRootNode(ToNode(_projectModel));
    }

    public void BumpVersions()
    {
        _tree.TraverseDfs(pm =>
        {
            if (pm.Version is not null)
                pm.Version = pm.Version.IncreasePatch();
        });
    }

    public void WriteBumpedVersionCsprojContents()
    {
        _tree.TraverseDfs(pm =>
        {
            var csprojPath = pm.Path;

            if (Path.IsPathRooted(csprojPath) is false)
            {
                csprojPath = Path.GetFullPath(csprojPath, SolutionPath);
            }

            WriteCsprojContent(csprojPath, pm.CsprojContent);
        });
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
        return await CsprojDependencyResolver.New().GetDependentProjectsAsync(_csprojPath, SolutionPath);
    }

    protected virtual void WriteCsprojContent(string csprojPath, string csprojContent)
    {
        File.WriteAllText(csprojPath, csprojContent);
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