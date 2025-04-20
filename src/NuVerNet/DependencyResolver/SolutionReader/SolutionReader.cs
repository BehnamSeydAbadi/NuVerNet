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

    public async Task<CsprojModel[]> GetCsprojsAsync()
    {
        var csprojModels = new List<CsprojModel>();

        foreach (var csprojPath in _csprojPaths)
        {
            var csprojContent = File.ReadAllText(csprojPath);

            csprojModels.Add(
                new CsprojModel(csprojPath, csprojContent, _solutionPath)
            );
        }

        await Task.CompletedTask;
        return csprojModels.ToArray();


        // var semaphoreSlim = new SemaphoreSlim(10);
        // await Task.CompletedTask;
        //
        // return _csprojPaths.Select(path =>
        // {
        //     // await semaphoreSlim.WaitAsync();
        //
        //     // try
        //     // {
        //     var content = File.ReadAllText(path);
        //     return new CsprojModel(path, content, _solutionPath);
        //     // }
        //     // finally
        //     // {
        //     //     semaphoreSlim.Release();
        //     // }
        // }).ToArray();
    }
}