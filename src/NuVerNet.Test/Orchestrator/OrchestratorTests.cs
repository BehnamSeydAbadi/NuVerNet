using FluentAssertions;
using NuVerNet.DependencyResolver;
using NuVerNet.Test.Orchestrator.Stubs;

// ReSharper disable InconsistentNaming

namespace NuVerNet.Test.Orchestrator;

public class OrchestratorTests
{
    [Fact(DisplayName =
        "There is a csproj content with version 0.0.1, When the version gets bumped, Then the version should be 0.0.2 successfully")]
    public async Task BumpVersionSuccessfullyAsync()
    {
        var projectA_Name = "ProjectA";
        var projectA_Path = $"..\\ProjectA\\{projectA_Name}.csproj";
        var projectA_AbsolutePath = @"C:\source\repos\ProjectA\src\ProjectA\ProjectA.csproj";
        var projectA_Version = "0.0.1";
        var projectA_BumpedVersion = "0.0.2";
        var projectA_Content =
            $@"
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                    <Version>{projectA_Version}</Version>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                </ItemGroup>
            </Project>
            ";

        var projectModel = new ProjectModel
        {
            Name = projectA_Name,
            Path = projectA_Path,
            AbsolutePath = projectA_AbsolutePath,
            Version = SemVersion.Parse(projectA_Version),
            CsprojContent = projectA_Content
        };

        var orchestrator = StubOrchestrator.New().WithProjectModel(projectModel);

        await orchestrator.LoadAsync();
        orchestrator.BumpVersions();

        projectModel.Version.ToString().Should().Be(projectA_BumpedVersion);
    }

    [Fact(DisplayName =
        "There is a bumped version csproj content, When the csproj content gets written, Then the csproj content should be written successfully")]
    public async Task WriteBumpedVersionCsprojContentsSuccessfullyAsync()
    {
        var projectA_Name = "ProjectA";
        var projectA_Path = $"..\\ProjectA\\{projectA_Name}.csproj";
        var projectA_AbsolutePath = @"C:\source\repos\ProjectA\src\ProjectA\ProjectA.csproj";
        var projectA_Content =
            $@"
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                    <Version>0.0.1</Version>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                </ItemGroup>
            </Project>
            ";
        
        var projectModel = new ProjectModel
        {
            Name = projectA_Name,
            Path = projectA_Path,
            AbsolutePath = projectA_AbsolutePath,
            Version = SemVersion.Parse("0.0.2"),
            CsprojContent = projectA_Content
        };

        var orchestrator = StubOrchestrator.New().WithProjectModel(projectModel);
        orchestrator.WithSolutionPath(@"C:\source\repos\ProjectA\src\ProjectA.sln");


        await orchestrator.LoadAsync();
        orchestrator.WriteBumpedVersionCsprojContents();

        orchestrator.CsprojAbsolutePath.Should().Be(projectA_AbsolutePath);
    }
}