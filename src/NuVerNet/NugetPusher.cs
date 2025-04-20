using System.Diagnostics;

namespace NuVerNet;

public class NugetPusher
{
    public async Task PushNugetAsync(string outputDirectory, string nugetServerUrl, string? apiKey = null)
    {
        var nupkgFiles = Directory.GetFiles(outputDirectory, "*.nupkg");

        if (nupkgFiles.Length == 0)
            throw new FileNotFoundException("No .nupkg file was found in the output directory.");

        foreach (var nupkgPath in nupkgFiles)
        {
            var pushCommand = string.IsNullOrWhiteSpace(apiKey)
                ? $@"nuget push ""{nupkgPath}"" --source ""{nugetServerUrl}"""
                : $@"nuget push ""{nupkgPath}"" --source ""{nugetServerUrl}"" --api-key ""{apiKey}""";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = pushCommand,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo)!;
            await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Push failed:\n{error}");
            }
        }
    }
}