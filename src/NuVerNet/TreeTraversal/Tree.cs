using NuVerNet.DependencyResolver.ViewModels;

namespace NuVerNet.TreeTraversal;

public class Tree
{
    private ProjectModel _rootNode;


    public static Tree New() => new();

    public Tree WithRootNode(ProjectModel rootNode)
    {
        _rootNode = rootNode;
        return this;
    }

    public void TraverseDfs(Action<ProjectModel> action)
    {
        TraverseDfs(_rootNode, action);
    }

    private void TraverseDfs(ProjectModel node, Action<ProjectModel> action)
    {
        action.Invoke(node);

        foreach (var childNode in node.UsedIn)
        {
            TraverseDfs(childNode, action);
        }
    }
}