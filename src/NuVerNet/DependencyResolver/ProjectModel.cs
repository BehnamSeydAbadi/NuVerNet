namespace NuVerNet.DependencyResolver;

public class ProjectModel
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string CsprojContent { get; set; }
    public required SemVersion? Version { get; set; }
    public List<ProjectModel> UsedIn { get; set; } = [];
}