using NuVerNet.DependencyResolver;
using NuVerNet.DependencyResolver.CsprojReader;
using NuVerNet.DependencyResolver.CsprojWriter;
using NuVerNet.Nuget;
using NuVerNet.TreeTraversal;

namespace NuVerNet;

public class Orchestrator
{
    private readonly Dictionary<string, string> _bumpedVersionProjectsIndexedByName = new();

    private string _csprojPath;
    private ProjectModel _projectModel;
    private Tree<ProjectModel> _tree;
    private string _outputPath;
    private string _nugetServerUrl;
    private string? _centralPackagePropsPath;
    private string? _apiKey;

    protected string SolutionPath;


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

    public Orchestrator WithOutputPath(string outputPath)
    {
        _outputPath = outputPath;
        return this;
    }

    public Orchestrator WithNugetServerUrl(string nugetServerUrl)
    {
        _nugetServerUrl = nugetServerUrl;
        return this;
    }

    public Orchestrator WithApiKey(string apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    public Orchestrator WithCentralPackagePropsPath(string centralPackagePropsPath)
    {
        _centralPackagePropsPath = centralPackagePropsPath;
        return this;
    }

    public void Load()
    {
        _projectModel = GetProjectModel();
        _tree = Tree<ProjectModel>.New().WithRootNode(ToNode(_projectModel));
    }


    public async Task BuildAndPublishNuGetPackagesAsync()
    {
        CentralPackageManagement.CentralPackageManagement? centralPackageManagement = null;

        if (string.IsNullOrWhiteSpace(_centralPackagePropsPath) is false)
        {
            centralPackageManagement = CentralPackageManagement.CentralPackageManagement.New();
            centralPackageManagement.WithCentralPackagePropsPath(_centralPackagePropsPath);
            centralPackageManagement.Load();
        }

        await _tree.TraverseDfsAsync(
            async pm =>
            {
                if (pm.Version is not null)
                {
                    pm.BumpVersion();
                    _bumpedVersionProjectsIndexedByName.Add(pm.Name, pm.Version.ToString());
                }

                WriteBumpedVersionCsprojContent(pm);

                var nugetPacker = NugetPacker.New();
                await nugetPacker.RestoreNugetPackagesAsync(pm);
                await nugetPacker.RebuildProjectAsync(pm);
                await nugetPacker.PackProjectAsync(pm, _outputPath);

                await new NugetPusher().PushNugetAsync(_outputPath, _nugetServerUrl, _apiKey);

                centralPackageManagement?.UpdatePackageVersion(pm.Name, pm.Version!.ToString());
            }
        );
    }


    protected virtual ProjectModel GetProjectModel()
    {
        return CsprojDependencyResolver.New().GetDependentProjects(_csprojPath, SolutionPath);
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

    private void WriteBumpedVersionCsprojContent(ProjectModel projectModel)
    {
        var csprojWriter = CsprojWriter.New()
            .WithCsprojContent(projectModel.CsprojContent)
            .WithProjectName(projectModel.Name);

        if (projectModel.Version is not null)
        {
            csprojWriter.WithVersion(projectModel.Version!.ToString());
        }

        var packageReferenceVersions = new List<PackageReferenceVersion>();

        var csprojReader = CsprojReader.New().WithCsprojContent(projectModel.CsprojContent);
        csprojReader.Load();

        foreach (var packageName in csprojReader.GetPackageReferencesNames())
        {
            _bumpedVersionProjectsIndexedByName.TryGetValue(packageName, out var version);

            if (version is not null)
            {
                packageReferenceVersions.Add(new PackageReferenceVersion(packageName, version));
            }
        }

        csprojWriter.WithPackageReferenceVersions(packageReferenceVersions.ToArray());

        csprojWriter.Write(projectModel.AbsolutePath);
    }
}