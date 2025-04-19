using NuVerNet.DependencyResolver;
using NuVerNet.TreeTraversal;

namespace NuVerNet.Versioning;

public class CsprojVersioner
{
    private CsprojDependencyResolver _csprojDependencyResolver;
    private Tree<ProjectModel> _tree;

    private CsprojVersioner()
    {
    }

    public static CsprojVersioner New() => new();

    public CsprojVersioner WithCsprojDependencyResolver(CsprojDependencyResolver csprojDependencyResolver)
    {
        _csprojDependencyResolver = csprojDependencyResolver;
        return this;
    }

    public CsprojVersioner WithTreeTraversal(Tree<ProjectModel> tree)
    {
        _tree = tree;
        return this;
    }

    public void BumpCsprojVersions(string csprojPath, string solutionPath)
    {
        var projectModel = _csprojDependencyResolver.GetDependentProjectsAsync(csprojPath, solutionPath);

        _tree.TraverseDfs(pm => pm.CsprojContent.BumpVersion());
    }
}