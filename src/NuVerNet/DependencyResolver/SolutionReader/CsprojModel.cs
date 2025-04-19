namespace NuVerNet.DependencyResolver.SolutionReader;

public class CsprojModel
{
    public CsprojModel(string path, string content, string solutionPath)
    {
        Path = path;

        AbsolutePath = System.IO.Path.IsPathRooted(path) is false
            ? System.IO.Path.GetFullPath(path, solutionPath)
            : path;

        Content = content;
    }

    public string Path { get; init; }
    public string AbsolutePath { get; init; }
    public string Content { get; init; }
}