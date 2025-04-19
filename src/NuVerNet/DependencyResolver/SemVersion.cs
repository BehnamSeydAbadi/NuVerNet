using NuGet.Versioning;

namespace NuVerNet.DependencyResolver;

public class SemVersion : IPrototype<SemVersion>
{
    private int _major;
    private int _minor;
    private int _patch;
    private string? _preRelease;
    private string? _buildMetadata;

    private SemVersion()
    {
    }

    public static SemVersion New() => new();

    public static SemVersion Parse(string version)
    {
        var semanticVersion = SemanticVersion.Parse(version);

        return New()
            .WithMajor(semanticVersion.Major).WithMinor(semanticVersion.Minor).WithPatch(semanticVersion.Patch)
            .WithPreRelease(semanticVersion.Release).WithBuildMetadata(semanticVersion.Metadata);
    }

    public SemVersion WithMajor(int major)
    {
        _major = major;
        return this;
    }

    public SemVersion WithMinor(int minor)
    {
        _minor = minor;
        return this;
    }

    public SemVersion WithPatch(int patch)
    {
        _patch = patch;
        return this;
    }

    public SemVersion WithPreRelease(string? preRelease)
    {
        _preRelease = preRelease;
        return this;
    }

    public SemVersion WithBuildMetadata(string? buildMetadata)
    {
        _buildMetadata = buildMetadata;
        return this;
    }

    public SemVersion Clone()
    {
        return new SemVersion
        {
            _major = this._major,
            _minor = this._minor,
            _patch = this._patch,
            _preRelease = this._preRelease,
            _buildMetadata = this._buildMetadata
        };
    }

    public override string ToString()
        => new SemanticVersion(_major, _minor, _patch, _preRelease, _buildMetadata).ToFullString();

    public SemVersion BumpMajor(int major) => this.Clone().WithMajor(major);
    public SemVersion BumpMinor(int minor) => this.Clone().WithMinor(minor);
    public SemVersion BumpPatch(int patch) => this.Clone().WithPatch(patch);
    public SemVersion BumpPreRelease(string preRelease) => this.Clone().WithPreRelease(preRelease);
    public SemVersion BumpBuildMetadata(string buildMetadata) => this.Clone().WithBuildMetadata(buildMetadata);

    public SemVersion IncreaseMajor() => this.Clone().WithMajor(this._major + 1);
    public SemVersion IncreaseMinor() => this.Clone().WithMinor(this._minor + 1);
    public SemVersion IncreasePatch() => this.Clone().WithPatch(this._patch + 1);
}