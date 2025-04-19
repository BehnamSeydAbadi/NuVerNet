using System.Diagnostics;
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
        _tree.TraverseDfs(pm => WriteCsprojContent(pm.AbsolutePath, pm.CsprojContent));
    }

    public async Task PackProjectsIntoNugetsAsync(string outputDirectory)
    {
        await _tree.TraverseDfsAsync(async pm =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $@"pack ""{pm.AbsolutePath}"" -o ""{outputDirectory}""",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(processStartInfo)!;
            await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"dotnet pack failed:\n{error}");
            }
        });
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