namespace NuVerNet.DependencyResolver.SolutionReader;

public class SolutionReader
{
    private string _solutionPath;
    private string[] _csprojPaths = [];

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
        var solutionDirectoryPath = Path.GetDirectoryName(_solutionPath)!;

        _csprojPaths = Directory.GetFiles(solutionDirectoryPath, "*.csproj", new EnumerationOptions
        {
            RecurseSubdirectories = true
        });
    }

    public CsprojModel[] GetCsprojs()
    {
        var csprojModels = new List<CsprojModel>();

        foreach (var csprojPath in _csprojPaths)
        {
            var csprojContent = File.ReadAllText(csprojPath);

            csprojModels.Add(
                new CsprojModel(csprojPath, csprojContent, _solutionPath)
            );
        }

        return csprojModels.ToArray();
    }
}