using NuVerNet.Abstract;

namespace NuVerNet.Nuget.Exceptions;

public class NupkgFileNotFoundException(string projectName) : AbstractException($"No .nupkg file was found for {projectName} project in the output directory.");