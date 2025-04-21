using NuVerNet.Abstract;

namespace NuVerNet.DependencyResolver.Exceptions;

public class MissingVersionException() : AbstractException("Version is missing in csproj content");