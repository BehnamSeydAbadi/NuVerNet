﻿using NuVerNet.DependencyResolver.CsprojReader;

namespace NuVerNet.Test.Stubs;

public class StubCsprojReader : CsprojReader
{
    public string ProjectName { get; private set; }
    public string ProjectPath { get; private set; }
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

    public StubCsprojReader WithProjectPath(string projectPath)
    {
        ProjectPath = projectPath;
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

    public override string? GetProjectVersion() => _projectVersion;
}