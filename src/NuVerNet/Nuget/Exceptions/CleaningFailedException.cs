using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class CleaningFailedException(string error) : AbstractException($"Cleaning failed: {error}");