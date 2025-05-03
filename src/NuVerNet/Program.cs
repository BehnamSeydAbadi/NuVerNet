using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using NuVerNet;

var rootCommand = new RootCommand("NuVerNet - Versioning Automation Tool");

var csprojPathOption = new Option<string>(
    name: "--csprojpath",
    description: "The path to the .csproj file")
{
    IsRequired = true
};

var solutionPathOption = new Option<string>(
    name: "--solutionpath",
    description: "The path to the .sln file")
{
    IsRequired = true
};

var outputPathOption = new Option<string>(
    name: "--outputpath",
    description: "The path where the .nupkg file will be saved")
{
    IsRequired = true
};

var nugetServerUrlOption = new Option<string>(
    name: "--nugetserverurl",
    description: "The URL of nuget server where the .nupkg will be accessible")
{
    IsRequired = true
};

var levelOption = new Option<string>(
    name: "--level",
    description: "The level to bump (major, minor, patch, etc)",
    getDefaultValue: () => "patch"
);

var bumpCommand = new Command("bump", "Bumps the version")
{
    csprojPathOption,
    solutionPathOption,
    outputPathOption,
    nugetServerUrlOption,
    levelOption
};

bumpCommand.SetHandler(
    async (string csprojPath, string solutionPath, string outputPath, string nugetServerUrl, string level) =>
    {
        var orchestrator = Orchestrator.New().WithCsprojPath(csprojPath).WithSolutionPath(solutionPath);

        ConsoleLog.Info($"Loading all csprojs from \"{solutionPath}\"");
        orchestrator.Load();
        ConsoleLog.Success("Loaded successfully");

        ConsoleLog.Info($"Bumping \"{level}\" level of versions of csprojs");
        orchestrator.BumpVersions();
        ConsoleLog.Success("Bumped successfully");

        ConsoleLog.Info($"Writing bumped version of csprojs");
        orchestrator.WriteBumpedVersionCsprojContents();
        ConsoleLog.Success("Written successfully");

        ConsoleLog.Info($"Packing projects into nuget and save them into \"{outputPath}\"");
        await orchestrator.PackProjectsIntoNugetsAsync(outputPath);
        ConsoleLog.Success("Packed successfully");

        ConsoleLog.Info($"Pushing nuget projects into \"{nugetServerUrl}\"");
        await orchestrator.PushProjectsToNugetServerAsync(outputPath, nugetServerUrl);
        ConsoleLog.Success("Pushed successfully");
    }, csprojPathOption, solutionPathOption, outputPathOption, nugetServerUrlOption, levelOption
);

rootCommand.AddCommand(bumpCommand);

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseExceptionHandler((exception, context) =>
    {
        ConsoleLog.Error($"Error: {exception.Message}");
        context.ExitCode = 1;
    });
var parser = builder.Build();

return await parser.InvokeAsync(args);