namespace NuVerNet.DependencyResolver.ViewModels;

public class ProjectModel
{
    public required string ProjectName { get; set; }
    public required string ProjectPath { get; set; }
    public required ProjectModel[] UsedIn { get; set; } = [];
}