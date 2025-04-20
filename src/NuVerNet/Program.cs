using System.CommandLine;
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
        Console.WriteLine($"📦 Bumping version of: {csprojPath}");
        Console.WriteLine($"🔧 Bump level: {level}");

        var orchestrator = Orchestrator.New().WithCsprojPath(csprojPath).WithSolutionPath(solutionPath);
        await orchestrator.LoadAsync();
        orchestrator.BumpVersions();
        orchestrator.WriteBumpedVersionCsprojContents();
        await orchestrator.PackProjectsIntoNugetsAsync(outputPath);
        await orchestrator.PushProjectsToNugetServerAsync(outputPath, nugetServerUrl);
        
    }, csprojPathOption, solutionPathOption, outputPathOption, nugetServerUrlOption, levelOption
);

rootCommand.AddCommand(bumpCommand);


return await rootCommand.InvokeAsync(args);
//
// try
// {
//     await rootCommand.InvokeAsync(args);
// }
// catch (Win32Exception ex) when (ex.Message.Contains("dotnet-suggest"))
// {
//     // Ignore this specific issue
// }