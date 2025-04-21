using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class PackingFailedException(string error) : AbstractException($"Packing failed: {error}");