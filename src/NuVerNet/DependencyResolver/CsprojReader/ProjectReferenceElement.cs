using System.Xml.Linq;

namespace NuVerNet.DependencyResolver.CsprojReader;

public class ProjectReferenceElement
{
    private XElement _xElement;

    private ProjectReferenceElement()
    {
    }

    public static ProjectReferenceElement New() => new();

    public ProjectReferenceElement WithXElement(XElement xElement)
    {
        _xElement = xElement;
        return this;
    }

    public string GetProjectName() => GetProjectPath().Replace(".csproj", string.Empty).Split("\\").Last();

    public string GetProjectPath() => _xElement.Attribute(XName.Get("Include"))!.Value;
}