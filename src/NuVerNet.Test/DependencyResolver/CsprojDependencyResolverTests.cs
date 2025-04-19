using FluentAssertions;
using NuVerNet.DependencyResolver;
using NuVerNet.Test.DependencyResolver.Stubs;

// ReSharper disable InconsistentNaming

namespace NuVerNet.Test.DependencyResolver;

public class CsprojDependencyResolverTests
{
    [Fact(DisplayName =
        "There is a .NET projects, When dependency logic works, Then only the project it'self with no UsedIn projects should be returned successfully")]
    public async Task DependencyLogicWorksOnOneProjectSuccessfully()
    {
        var projectA_Name = "ProjectA";
        var projectA_Path = $"..\\ProjectA\\{projectA_Name}.csproj";
        var projectA_Version = "0.0.1";
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
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>
            </Project>
            ";

        var dependency = StubCsprojDependencyResolver.New().WithProjectModel(new ProjectModel
        {
            Name = projectA_Name,
            Path = projectA_Path,
            CsprojContent = projectA_Content,
            Version = SemVersion.Parse(projectA_Version),
        });

        var projectModel = await dependency.GetDependentProjectsAsync(projectA_Path, solutionPath: string.Empty);

        projectModel.Name.Should().Be(projectA_Name);
        projectModel.Path.Should().Be(projectA_Path);
        projectModel.CsprojContent.Should().Be(projectA_Content);
        projectModel.Version!.ToString().Should().Be(projectA_Version);
        projectModel.UsedIn.Should().BeEmpty();
    }

    [Fact(DisplayName =
        "There are two .NET projects which one of them is dependent to another one, When dependency logic works, Then the dependent project should be returned successfully")]
    public async Task DependencyLogicWorksOnTwoDependentProjectsSuccessfully()
    {
        var projectA_Name = "ProjectA";
        var projectA_Path = $"..\\ProjectA\\{projectA_Name}.csproj";
        var projectA_Version = "0.0.1";
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
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>
            </Project>
            ";


        var projectB_Name = "ProjectB";
        var projectB_Path = $"..\\ProjectB\\{projectB_Name}.csproj";
        var projectB_Version = "0.0.2";
        var projectB_Content =
            $@"
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                    <Version>{projectB_Version}</Version>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>

                <ItemGroup>
                  <ProjectReference Include=""{projectA_Path}"" />
                </ItemGroup>
            </Project>
            ";

        var dependency = StubCsprojDependencyResolver.New()
            .WithProjectModel(new ProjectModel
            {
                Name = projectA_Name,
                Path = projectA_Path,
                CsprojContent = projectA_Content,
                Version = SemVersion.Parse(projectA_Version),
            })
            .WithCsprojContentsOfSolution(
                new StubCsprojDependencyResolver.Csproj(projectA_Path, projectA_Content),
                new StubCsprojDependencyResolver.Csproj(projectB_Path, projectB_Content)
            )
            .WithCsprojReaders(
                StubCsprojReader.New()
                    .WithProjectName(projectA_Name)
                    .WithProjectPath(projectA_Path)
                    .WithProjectContent(projectA_Content)
                    .WithProjectVersion(projectA_Version),
                StubCsprojReader.New()
                    .WithProjectName(projectB_Name)
                    .WithProjectPath(projectB_Path)
                    .WithProjectContent(projectB_Content)
                    .WithProjectVersion(projectB_Version)
                    .WithProjectReferences(projectA_Name)
            );


        var projectModel = await dependency.GetDependentProjectsAsync(projectA_Path, solutionPath: string.Empty);

        projectModel.Name.Should().Be(projectA_Name);
        projectModel.Path.Should().Be(projectA_Path);
        projectModel.CsprojContent.Should().Be(projectA_Content);
        projectModel.Version!.ToString().Should().Be(projectA_Version);
        projectModel.UsedIn.Count.Should().Be(1);

        projectModel.UsedIn[0].Name.Should().Be(projectB_Name);
        projectModel.UsedIn[0].Path.Should().Be(projectB_Path);
        projectModel.UsedIn[0].CsprojContent.Should().Be(projectB_Content);
        projectModel.UsedIn[0].Version!.ToString().Should().Be(projectB_Version);
        projectModel.UsedIn[0].UsedIn.Should().BeEmpty();
    }

    [Fact(DisplayName =
        "There are multiple .NET projects which are dependent to each other hierarchically, When dependency logic works, Then the dependent project should be returned successfully")]
    public async Task DependencyLogicWorksOnThreeDependentProjectsSuccessfully()
    {
        var projectA_Name = "ProjectA";
        var projectA_Path = $"..\\ProjectA\\{projectA_Name}.csproj";
        var projectA_Version = "0.0.1";
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
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>
            </Project>
            ";


        var projectB_Name = "ProjectB";
        var projectB_Path = $"..\\ProjectB\\{projectB_Name}.csproj";
        var projectB_Version = "0.0.2";
        var projectB_Content =
            $@"
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                    <Version>{projectB_Version}</Version>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>

                <ItemGroup>
                  <ProjectReference Include=""{projectA_Path}"" />
                </ItemGroup>
            </Project>
            ";


        var projectC_Name = "ProjectC";
        var projectC_Path = $"..\\ProjectC\\{projectC_Name}.csproj";
        var projectC_Version = "0.0.3";
        var projectC_Content =
            $@"
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                    <Version>{projectC_Version}</Version>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>

                <ItemGroup>
                  <ProjectReference Include=""{projectB_Path}"" />
                </ItemGroup>
            </Project>
            ";

        var dependency = StubCsprojDependencyResolver.New()
            .WithProjectModel(new ProjectModel
            {
                Name = projectA_Name,
                Path = projectA_Path,
                CsprojContent = projectA_Content,
                Version = SemVersion.Parse(projectA_Version),
            })
            .WithCsprojContentsOfSolution(
                new StubCsprojDependencyResolver.Csproj(projectA_Path, projectA_Content),
                new StubCsprojDependencyResolver.Csproj(projectB_Path, projectB_Content),
                new StubCsprojDependencyResolver.Csproj(projectC_Path, projectC_Content)
            )
            .WithCsprojReaders(
                StubCsprojReader.New()
                    .WithProjectName(projectA_Name)
                    .WithProjectPath(projectA_Path)
                    .WithProjectContent(projectA_Content)
                    .WithProjectVersion(projectA_Version),
                StubCsprojReader.New()
                    .WithProjectName(projectB_Name)
                    .WithProjectPath(projectB_Path)
                    .WithProjectContent(projectB_Content)
                    .WithProjectVersion(projectB_Version)
                    .WithProjectReferences(projectA_Name),
                StubCsprojReader.New()
                    .WithProjectName(projectC_Name)
                    .WithProjectPath(projectC_Path)
                    .WithProjectContent(projectC_Content)
                    .WithProjectVersion(projectC_Version)
                    .WithProjectReferences(projectB_Name)
            );

        var projectA_projectModel = await dependency.GetDependentProjectsAsync(
            projectA_Path, solutionPath: string.Empty
        );

        projectA_projectModel.Name.Should().Be(projectA_Name);
        projectA_projectModel.Path.Should().Be(projectA_Path);
        projectA_projectModel.CsprojContent.Should().Be(projectA_Content);
        projectA_projectModel.Version!.ToString().Should().Be(projectA_Version);
        projectA_projectModel.UsedIn.Count.Should().Be(1);

        var projectB_projectModel = projectA_projectModel.UsedIn[0];
        projectB_projectModel.Name.Should().Be(projectB_Name);
        projectB_projectModel.Path.Should().Be(projectB_Path);
        projectB_projectModel.CsprojContent.Should().Be(projectB_Content);
        projectB_projectModel.Version!.ToString().Should().Be(projectB_Version);
        projectB_projectModel.UsedIn.Count.Should().Be(1);

        var projectC_projectModel = projectB_projectModel.UsedIn[0];
        projectC_projectModel.Name.Should().Be(projectC_Name);
        projectC_projectModel.Path.Should().Be(projectC_Path);
        projectC_projectModel.CsprojContent.Should().Be(projectC_Content);
        projectC_projectModel.Version!.ToString().Should().Be(projectC_Version);
        projectC_projectModel.UsedIn.Should().BeEmpty();
    }
}