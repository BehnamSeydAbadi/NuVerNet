namespace NuVerNet.DependencyResolver;

public class SolutionReader
{
    private string _solutionPath;
    private string[] _csprojPaths;

    private SolutionReader()
    {
    }

    public static SolutionReader New() => new();

    public SolutionReader WithSolutionPath(string solutionPath)
    {
        _solutionPath = solutionPath;
        return this;
    }

    public void Load()
    {
        _csprojPaths = Directory.GetFiles(_solutionPath, "*.csproj", new EnumerationOptions
        {
            RecurseSubdirectories = true
        });
    }

    public async Task<string[]> GetCsprojContentsAsync()
    {
        return await Task.WhenAll(
            _csprojPaths.Select(p => File.ReadAllTextAsync(p))
        );
    }
}