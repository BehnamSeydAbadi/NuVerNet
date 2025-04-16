using System.Xml.Linq;

namespace NuVerNet.DependencyResolver.Extensions;

public static class XElementExtensions
{
    public static string GetProjectName(this XElement projectReferenceElement)
        => projectReferenceElement.GetRelativeProjectPath().GetProjectName();

    public static string GetRelativeProjectPath(this XElement projectReferenceElement)
        => projectReferenceElement.Attribute(XName.Get("Include"))!.Value;
}