namespace NuVerNet.DependencyResolver.Extensions;

public static class StringExtensions
{
    public static string GetProjectName(this string csprojPath)
        => csprojPath.Replace(".csproj", string.Empty).Split("\\").Last();
}