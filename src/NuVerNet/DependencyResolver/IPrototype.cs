namespace NuVerNet.DependencyResolver;

public interface IPrototype<T> where T : class
{
    T Clone();
}