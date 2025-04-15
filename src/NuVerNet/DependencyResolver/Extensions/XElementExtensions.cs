using System.Xml.Linq;

namespace NuVerNet.DependencyResolver.Extensions;

public static class XElementExtensions
{
    public static string GetProjectName(this XElement projectReferenceElement)
    {
        var csprojRelativePath = projectReferenceElement.GetRelativeProjectPath();
        return csprojRelativePath.Replace(".csproj", string.Empty).Split("\\").Last();
    }

    public static string GetRelativeProjectPath(this XElement projectReferenceElement)
    {
        return projectReferenceElement.Attribute(XName.Get("Include"))!.Value;
    }
}