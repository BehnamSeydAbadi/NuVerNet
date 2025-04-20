namespace NuVerNet.TreeTraversal;

public class Node<T> where T : class
{
    private List<Node<T>> _children = [];

    public Node(T @object)
    {
        Object = @object;
    }

    public T Object { get; }
    public IReadOnlyList<Node<T>> Children => _children;

    public void AddChild(Node<T> child) => _children.Add(child);
}