using System.Diagnostics;
using NuVerNet.DependencyResolver;

namespace NuVerNet;

public class NugetPacker
{
    private NugetPacker()
    {
    }

    public static NugetPacker New() => new();

    public async Task PackProjectsIntoNugetsAsync(ProjectModel projectModel, string outputDirectory)
    {
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
        await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"dotnet pack failed:\n{error}");
        }
    }
}