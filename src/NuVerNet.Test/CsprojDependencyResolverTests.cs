using System.Xml.Linq;
using FluentAssertions;

namespace NuVerNet.Test;

public class CsprojDependencyResolverTests
{
    [Fact(DisplayName =
        "There is a .NET projects, When dependency logic works, Then null should be returned successfully")]
    public async Task DependencyLogicWorksOnOneProjectSuccessfully()
    {
        var csprojContent =
            @"
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>
            </Project>
            ";

        var dependency = StubCsprojDependencyResolver.New().WithCsprojContent(csprojContent);

        var dependentProjects = await dependency.GetDependentProjectsAsync(csprojPath: string.Empty);

        dependentProjects.Should().BeEmpty();
    }

    [Fact(DisplayName =
        "There are two .NET projects which one of them is dependent to another one, When dependency logic works, Then the dependent project should be returned successfully")]
    public async Task DependencyLogicWorksOnTwoDependentProjectsSuccessfully()
    {
        var dependentProjectName = "NuVerNet";
        var relativeDependentProjectPath = $"..\\NuVerNet\\{dependentProjectName}.csproj";

        var csprojContent =
            @$"
            <Project Sdk=""Microsoft.NET.Sdk"">
            
                <PropertyGroup>
                    <TargetFramework>net8.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
            
                    <IsPackable>false</IsPackable>
                    <IsTestProject>true</IsTestProject>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""coverlet.collector"" Version=""6.0.0""/>
                    <PackageReference Include=""FluentAssertions"" Version=""8.2.0"" />
                    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""17.8.0""/>
                    <PackageReference Include=""xunit"" Version=""2.5.3""/>
                    <PackageReference Include=""xunit.runner.visualstudio"" Version=""2.5.3""/>
                </ItemGroup>
            
                <ItemGroup>
                    <Using Include=""Xunit""/>
                </ItemGroup>
            
                <ItemGroup>
                  <ProjectReference Include=""{relativeDependentProjectPath}"" />
                </ItemGroup>
            
            </Project>
            ";

        var dependency = StubCsprojDependencyResolver.New().WithCsprojContent(csprojContent);

        var dependentProjects = await dependency.GetDependentProjectsAsync(csprojPath: string.Empty);

        dependentProjects.Should().NotBeEmpty();
        dependentProjects[0].ProjectName.Should().Be(dependentProjectName);
        dependentProjects[0].ProjectPath.Should().Be(relativeDependentProjectPath);
        dependentProjects[0].UsedIn.Should().BeEmpty();
    }

    [Fact(DisplayName =
        "There are multiple .NET projects which are dependent to each other hierarchically, When dependency logic works, Then the dependent project should be returned successfully")]
    public async Task DependencyLogicWorksOnThreeDependentProjectsSuccessfully()
    {
        var csprojContent = @"
        <Project Sdk=""Microsoft.NET.Sdk"">
        
            <PropertyGroup>
                <Version>0.0.2</Version>
                <PackageId>ERPCore.Utility.Domain</PackageId>
                <PackageTags>erp;utility;domain</PackageTags>
                <PackageOutputPath>../../localPackages/ERPCore.Utility.Domain</PackageOutputPath>
            </PropertyGroup>
        
            <ItemGroup>
                <None Include=""README.md"" Pack=""true"" PackagePath=""""/>
            </ItemGroup>
        
            <ItemGroup>
                <PackageReference Include=""Mapster""/>
                <PackageReference Include=""Quantum.Domain""/>
            </ItemGroup>
        
            <ItemGroup>
                <Content Include=""Utility.ErrorMessage.json"" CopyToPublishDirectory=""PreserveNewest"" CopyToOutputDirectory=""PreserveNewest""/>
            </ItemGroup>
        
            <Target Name=""CopyDllAfterBuild"" AfterTargets=""Build"">
                <Copy
                        SourceFiles=""$(TargetDir)$(AssemblyName).dll""
                        DestinationFolder=""..\..\..\..\Build""/>
            </Target>
        
        </Project>
        ";

        string[] csprojPaths =
        [
            @"
            <Project Sdk=""Microsoft.NET.Sdk"">
            
                <PropertyGroup>
                    <Version>0.0.2</Version>
                    <PackageId>ERPCore.Utility.ApplicationService</PackageId>
                    <PackageTags>erp;utility;application;service;applicationservice</PackageTags>
                    <PackageOutputPath>../../localPackages/ERPCore.Utility.ApplicationService</PackageOutputPath>
                </PropertyGroup>
            
                <ItemGroup>
                    <None Include=""README.md"" Pack=""true"" PackagePath=""""/>
                </ItemGroup>
            
                <ItemGroup>
                    <ProjectReference Include=""..\ERPCore.Utility.Domain\ERPCore.Utility.Domain.csproj""/>
                </ItemGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Quantum.Dispatcher""/>
                </ItemGroup>
            
                <Target Name=""CopyDllAfterBuild"" AfterTargets=""Build"">
                    <Copy
                            SourceFiles=""$(TargetDir)$(AssemblyName).dll""
                            DestinationFolder=""..\..\..\..\Build""/>
                </Target>
            
            </Project>",
            @"
            <Project Sdk=""Microsoft.NET.Sdk"">
            
                <PropertyGroup>
                    <Version>0.0.3</Version>
                    <PackageId>ERPCore.Utility.Infrastructure</PackageId>
                    <PackageTags>erp;utility;infrastructure</PackageTags>
                    <PackageOutputPath>../../localPackages/ERPCore.Utility.Infrastructure</PackageOutputPath>
                </PropertyGroup>
            
                <ItemGroup>
                    <None Include=""README.md"" Pack=""true"" PackagePath=""""/>
                </ItemGroup>
                
                <ItemGroup>
                    <ProjectReference Include=""..\ERPCore.Utility.ApplicationService\ERPCore.Utility.ApplicationService.csproj""/>
                </ItemGroup>
            
                <Target Name=""CopyDllAfterBuild"" AfterTargets=""Build"">
                    <Copy
                            SourceFiles=""$(TargetDir)$(AssemblyName).dll""
                            DestinationFolder=""..\..\..\..\Build""/>
                </Target>
            
            </Project>"
        ];

        var dependency = StubCsprojDependencyResolver.New().WithCsprojContent(csprojContent).WithCsprojContentsOfSolution(csprojPaths);

        var dependentProjects = await dependency.GetDependentProjectsAsync(csprojPath: string.Empty, solutionPath: string.Empty);

        dependentProjects.Length.Should().Be(3);

        var utilityDomainProjectModel = dependentProjects.SingleOrDefault(dp => dp.ProjectName == "ERPCore.Utility.Domain");
        utilityDomainProjectModel.Should().NotBeNull();
        utilityDomainProjectModel.ProjectPath.Should().Be(@"..\ERPCore.Utility.Domain\ERPCore.Utility.Domain.csproj");
        //TODO: And so on        
    }
}