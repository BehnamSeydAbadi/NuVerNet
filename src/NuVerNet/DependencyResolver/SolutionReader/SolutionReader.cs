namespace NuVerNet.DependencyResolver.SolutionReader;

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

    public async Task<CsprojModel[]> GetCsprojsAsync()
    {
        var semaphoreSlim = new SemaphoreSlim(10);

        var tasks = _csprojPaths.Select(async path =>
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                var content = await File.ReadAllTextAsync(path);
                return new CsprojModel(path, content);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        });

        return await Task.WhenAll(tasks);
    }


    // public async Task<CsprojModel[]> GetCsprojsAsync()
    // {
    //     var csprojModels = new List<CsprojModel>();
    //
    //     foreach (var csprojPath in _csprojPaths)
    //     {
    //         var csprojContent = await File.ReadAllTextAsync(csprojPath);
    //         csprojModels.Add(new CsprojModel(csprojPath, csprojContent));
    //     }
    //
    //     return csprojModels.ToArray();
    // }
}