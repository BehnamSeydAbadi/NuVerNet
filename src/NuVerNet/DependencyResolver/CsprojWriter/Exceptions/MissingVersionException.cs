using NuVerNet.Abstract;

namespace NuVerNet.DependencyResolver.CsprojWriter.Exceptions;

public class MissingVersionException(string name) : AbstractException($"Version is missing in csproj content \"{name}\"");