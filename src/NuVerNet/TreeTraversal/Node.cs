namespace NuVerNet.TreeTraversal;

public class Node<T> where T : class
{
    public Node(T @object, Node<T> child)
    {
        Object = @object;
        Child = child;
    }

    public T Object { get; }
    public Node<T> Child { get; }
}