using NuVerNet.DependencyResolver;
using NuVerNet.DependencyResolver.CsprojReader;

namespace NuVerNet.Test.DependencyResolver.Stubs;

public class StubCsprojReader : CsprojReader
{
    public string ProjectName { get; private set; }
    public string ProjectPath { get; private set; }
    public string AbsoluteProjectPath { get; private set; }
    public string ProjectContent { get; private set; }
    private string _projectVersion;
    private HashSet<string> _projectReferenceNames = [];

    private StubCsprojReader() : base()
    {
    }

    public new static StubCsprojReader New() => new();

    public StubCsprojReader WithProjectName(string projectName)
    {
        ProjectName = projectName;
        return this;
    }

    public StubCsprojReader WithRelativeProjectPath(string relativeProjectPath)
    {
        ProjectPath = relativeProjectPath;
        return this;
    }

    public StubCsprojReader WithAbsoluteProjectPath(string absoluteProjectPath)
    {
        AbsoluteProjectPath = absoluteProjectPath;
        return this;
    }


    public StubCsprojReader WithProjectContent(string projectContent)
    {
        ProjectContent = projectContent;
        return this;
    }

    public StubCsprojReader WithProjectVersion(string projectVersion)
    {
        _projectVersion = projectVersion;
        return this;
    }

    public StubCsprojReader WithProjectReferences(params string[] projectNames)
    {
        _projectReferenceNames = projectNames.ToHashSet();
        return this;
    }


    public override void Load()
    {
    }

    public override bool HasProjectReference(string projectName) => _projectReferenceNames.Contains(projectName);

    public override string GetProjectName() => ProjectName;

    public override SemVersion? GetProjectVersion() => SemVersion.Parse(_projectVersion);

    public override string GetContent() => ProjectContent;


    protected override string GetRelativePath() => ProjectPath;
    protected override string GetAbsolutePath() => AbsoluteProjectPath;
}