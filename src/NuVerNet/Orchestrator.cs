using System.Diagnostics;
using NuVerNet.DependencyResolver;
using NuVerNet.DependencyResolver.CsprojReader;
using NuVerNet.Nuget;
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

    public void Load()
    {
        _projectModel = GetProjectModel();
        _tree = Tree<ProjectModel>.New().WithRootNode(ToNode(_projectModel));
    }

    public void BumpVersions()
    {
        _tree.TraverseDfs(pm =>
        {
            if (pm.Version is not null)
                pm.BumpVersion();
        });
    }

    public void WriteBumpedVersionCsprojContents()
    {
        _tree.TraverseDfs(pm =>
        {
            if (pm.Version is null) return;

            var bumpedVersionCsprojContent = GetBumpVersionedCsprojContent(pm);
            WriteCsprojContent(pm.AbsolutePath, bumpedVersionCsprojContent);
        });
    }

    public async Task PackProjectsIntoNugetsAsync(string outputDirectory)
    {
        await _tree.TraverseDfsAsync(
            async pm => await NugetPacker.New().PackProjectsIntoNugetsAsync(pm, outputDirectory)
        );
    }

    public async Task PushProjectsToNugetServerAsync(string outputDirectory, string nugetServerUrl, string? apiKey = null)
    {
        await new NugetPusher().PushNugetAsync(outputDirectory, nugetServerUrl, apiKey);
    }


    protected virtual ProjectModel GetProjectModel()
    {
        return CsprojDependencyResolver.New().GetDependentProjects(_csprojPath, SolutionPath);
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
            var childNode = ToNode(usedInProjectModel);
            node.AddChild(childNode);
        }

        return node;
    }

    private string GetBumpVersionedCsprojContent(ProjectModel projectModel)
    {
        var csprojReader = CsprojReader.New().WithCsprojContent(projectModel.CsprojContent);
        csprojReader.Load();
        csprojReader.SetVersion(projectModel.Version!.ToString());
        return csprojReader.GetBumpedVersionCsprojContent()!;
    }
}