using System.Diagnostics;
using System.Drawing;
using NuVerNet.DependencyResolver;
using NuVerNet.Nuget.Exceptions;

namespace NuVerNet.Nuget;

public class NugetPacker
{
    private NugetPacker()
    {
    }

    public static NugetPacker New() => new();

    public async Task PackProjectsIntoNugetsAsync(ProjectModel projectModel, string outputDirectory)
    {
        ConsoleLog.Info($"Packing {projectModel.Name}...");

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $@"pack ""{projectModel.AbsolutePath}"" -o ""{outputDirectory}""",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(processStartInfo)!;
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new PackingFailedException(string.IsNullOrWhiteSpace(error) is false ? error : output);
        }

        ConsoleLog.Success($"{projectModel.Name} packed successfully");
    }
}