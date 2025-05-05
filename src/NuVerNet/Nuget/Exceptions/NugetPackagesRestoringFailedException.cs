using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class NugetPackagesRestoringFailedException(string error) : AbstractException($"Building failed: {error}");