using NuVerNet.Abstract;

namespace NuVerNet.DependencyResolver.CsprojReader.Exceptions;

public class SourceNotFoundException : AbstractException
{
    public SourceNotFoundException() : base("Either csprojPath or csprojContent should be provided.")
    {
    }
}