using FluentAssertions;
using NuVerNet.DependencyResolver;

namespace NuVerNet.Test.CsprojContentModel;

public class CsprojContentModelTests
{
    [Fact(DisplayName =
        "There is a csproj content with version tag, When it gets parsed, Then version of the csproj content should be returned successfully")]
    public void GetVersionFromCsprojContentWhenItExistsSuccessfully()
    {
        var version = "0.0.1";

        var csprojContentString = $@"            
            <Project Sdk=""Microsoft.NET.Sdk.Worker"">
        
                <PropertyGroup>
                    <TargetFramework>net7.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <UserSecretsId>dotnet-WindowsService-0001F3E8-FC7E-4AB5-9664-D579AB860F6A</UserSecretsId>
                    <Version>{version}</Version>
                </PropertyGroup>
            
                <ItemGroup>
                    <PackageReference Include=""Microsoft.Extensions.Hosting"" Version=""7.0.1""/>
                    <PackageReference Include=""Microsoft.Extensions.Hosting.WindowsServices"" Version=""7.0.1"" />
                    <PackageReference Include=""Microsoft.Extensions.Logging.EventLog"" Version=""8.0.0"" />
                    <PackageReference Include=""Quantum.Logging"" Version=""0.0.7"" />
                </ItemGroup>
            </Project>";

        var csprojContent = new CsprojContent(csprojContentString);

        csprojContent.GetVersion().Should().Be(version);
    }

    [Fact(DisplayName =
        "There is a csproj content without version tag, When it gets parsed, Then nothing should be returned when GetVersion called")]
    public void GetVersionShouldReturnNullFromCsprojContentWhenItDoesNotExistSuccessfully()
    {
        var csprojContentString = $@"            
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
            </Project>";

        var csprojContent = new CsprojContent(csprojContentString);

        csprojContent.GetVersion().Should().BeNullOrEmpty();
    }
}