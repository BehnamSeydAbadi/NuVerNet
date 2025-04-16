using NuVerNet.DependencyResolver;

namespace NuVerNet.Test;

public class StubCsprojDependencyResolver : CsprojDependencyResolver
{
    private string _csprojContent;
    private string[] _csprojContents;

    private StubCsprojDependencyResolver()
    {
    }

    public static StubCsprojDependencyResolver New() => new();

    public StubCsprojDependencyResolver WithCsprojContent(string csprojContent)
    {
        _csprojContent = csprojContent;
        return this;
    }

    public StubCsprojDependencyResolver WithCsprojContentsOfSolution(params string[] csprojContents)
    {
        _csprojContents = csprojContents;
        return this;
    }

    protected override async Task<string> GetCsprojContentAsync(string csprojPath)
    {
        await Task.CompletedTask;
        return _csprojContent;
    }

    protected override async Task<string[]> GetCsprojContentsOfSolutionAsync(string solutionPath)
    {
        await Task.CompletedTask;
        return _csprojContents;
    }
}