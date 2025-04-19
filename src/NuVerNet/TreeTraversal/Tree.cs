namespace NuVerNet.TreeTraversal;

public class Tree<T> where T : class
{
    private Node<T>? _rootNode;


    public static Tree<T> New() => new();

    public Tree<T> WithRootNode(Node<T> rootNode)
    {
        _rootNode = rootNode;
        return this;
    }

    public void TraverseDfs(Action<T> action)
    {
        if (_rootNode is null)
            throw new InvalidOperationException("Root node is null");

        TraverseDfs(_rootNode, action);
    }

    public async Task TraverseDfsAsync(Func<T, Task> action)
    {
        if (_rootNode is null)
            throw new InvalidOperationException("Root node is null");

        await TraverseDfsAsync(_rootNode, action);
    }

    private void TraverseDfs(Node<T> node, Action<T> action)
    {
        action.Invoke(node.Object);

        foreach (var childNode in node.Children)
        {
            TraverseDfs(childNode, action);
        }
    }

    private async Task TraverseDfsAsync(Node<T> node, Func<T, Task> action)
    {
        await action.Invoke(node.Object);

        foreach (var childNode in node.Children)
        {
            await TraverseDfsAsync(childNode, action);
        }
    }
}