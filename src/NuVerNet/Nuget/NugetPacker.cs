using System.Diagnostics;
using NuVerNet.DependencyResolver;
using NuVerNet.Nuget.Exceptions;

namespace NuVerNet.Nuget;

public class NugetPacker
{
    private NugetPacker()
    {
    }

    public static NugetPacker New() => new();

    public async Task PackProjectAsync(ProjectModel projectModel, string outputDirectory)
    {
        ConsoleLog.Info($"Packing {projectModel.Name}...");

        var errorMessage = await ProcessExecuterAsync(
            fileName: "dotnet", arguments: $@"pack ""{projectModel.AbsolutePath}"" -o ""{outputDirectory}"""
        );

        if (string.IsNullOrWhiteSpace(errorMessage) is false)
        {
            throw new PackingFailedException(errorMessage);
        }

        ConsoleLog.Success($"{projectModel.Name} packed successfully");
    }

    public async Task RestoreNugetPackagesAsync(ProjectModel projectModel)
    {
        ConsoleLog.Info($"Restoring nuget packages of {projectModel.Name}...");

        var errorMessage = await ProcessExecuterAsync(
            fileName: "dotnet", arguments: $@"restore ""{projectModel.AbsolutePath}"" -warnaserror:false -nowarn:""NU1507;NU1803"""
        );

        if (string.IsNullOrWhiteSpace(errorMessage) is false)
        {
            throw new NugetPackagesRestoringFailedException(errorMessage);
        }

        ConsoleLog.Success($"{projectModel.Name} nuget packages restored successfully");
    }

    public async Task BuildProjectAsync(ProjectModel projectModel)
    {
        ConsoleLog.Info($"Building {projectModel.Name}...");

        var errorMessage = await ProcessExecuterAsync(
            fileName: "dotnet", arguments: $@"build ""{projectModel.AbsolutePath}"""
        );

        if (string.IsNullOrWhiteSpace(errorMessage) is false)
        {
            throw new BuildingFailedException(errorMessage);
        }

        ConsoleLog.Success($"{projectModel.Name} built successfully");
    }

    public async Task CleanProjectAsync(ProjectModel projectModel)
    {
        ConsoleLog.Info($"Cleaning {projectModel.Name}...");

        var errorMessage = await ProcessExecuterAsync(
            fileName: "dotnet", arguments: $@"clean ""{projectModel.AbsolutePath}"""
        );

        if (string.IsNullOrWhiteSpace(errorMessage) is false)
        {
            throw new CleaningFailedException(errorMessage);
        }

        ConsoleLog.Success($"{projectModel.Name} cleaned successfully");
    }

    public async Task RebuildProjectAsync(ProjectModel projectModel)
    {
        await CleanProjectAsync(projectModel);
        await BuildProjectAsync(projectModel);
    }


    private async Task<string> ProcessExecuterAsync(string fileName, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = Process.Start(processStartInfo)!;
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            return string.IsNullOrWhiteSpace(error) ? output : error + Environment.NewLine + output;
        }

        return string.Empty;
    }
}