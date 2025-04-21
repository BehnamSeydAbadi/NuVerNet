using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class PushingFailedException(string error) : AbstractException($"Pushing failed: {error}");