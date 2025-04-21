using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class NupkgFileNotFoundException() : AbstractException("No .nupkg file was found in the output directory.");