using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class BuildingFailedException(string error) : AbstractException($"Building failed: {error}");