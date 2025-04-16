namespace NuVerNet.DependencyResolver.ViewModels;

public class ProjectModel
{
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string? Version { get; set; }
    public ProjectModel[] UsedIn { get; set; } = [];
}