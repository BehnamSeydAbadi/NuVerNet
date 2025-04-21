using NuVerNet.DependencyResolver.Exceptions;

namespace NuVerNet.DependencyResolver;

public class ProjectModel
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string AbsolutePath { get; set; }
    public required string CsprojContent { get; set; }
    public required SemVersion? Version { get; set; }
    public List<ProjectModel> UsedIn { get; set; } = [];

    public void BumpVersion()
    {
        if (Version is null)
            throw new MissingVersionException();

        Version = Version.IncreasePatch();
    }
}