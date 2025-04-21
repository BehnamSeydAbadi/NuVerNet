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

        ConsoleWrite($"Loading all csprojs from \"{solutionPath}\"");
        orchestrator.Load();
        ConsoleWrite("Loaded successfully", ConsoleColor.Green);

        ConsoleWrite($"Bumping \"{level}\" level of versions of csprojs");
        orchestrator.BumpVersions();
        ConsoleWrite("Bumped successfully", ConsoleColor.Green);

        ConsoleWrite($"Writing bumped version of csprojs");
        orchestrator.WriteBumpedVersionCsprojContents();
        ConsoleWrite("Written successfully", ConsoleColor.Green);

        ConsoleWrite($"Packing projects into nuget and save them into \"{outputPath}\"");
        await orchestrator.PackProjectsIntoNugetsAsync(outputPath);
        ConsoleWrite("Packed successfully", ConsoleColor.Green);

        ConsoleWrite($"Pushing nuget projects into \"{nugetServerUrl}\"");
        await orchestrator.PushProjectsToNugetServerAsync(outputPath, nugetServerUrl);
        ConsoleWrite("Pushed successfully", ConsoleColor.Green);
    }, csprojPathOption, solutionPathOption, outputPathOption, nugetServerUrlOption, levelOption
);

rootCommand.AddCommand(bumpCommand);

var builder = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseExceptionHandler((exception, context) =>
    {
        ConsoleWrite($"Error: {exception.Message}", ConsoleColor.Red);
        context.ExitCode = 1;
    });
var parser = builder.Build();

return await parser.InvokeAsync(args);


void ConsoleWrite(string message, ConsoleColor color = ConsoleColor.White)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}