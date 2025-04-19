using System.Xml.Linq;

namespace NuVerNet.DependencyResolver.CsprojReader;

public class CsprojReader
{
    private string _csprojPath;
    private CsprojContent _csprojContent;
    private XDocument _csprojXdocument;

    protected CsprojReader()
    {
    }

    public static CsprojReader New() => new();

    public CsprojReader WithCsprojPath(string csprojPath)
    {
        _csprojPath = csprojPath;
        _csprojContent = new CsprojContent(File.ReadAllText(csprojPath));
        return this;
    }

    public virtual void Load()
    {
        if (string.IsNullOrWhiteSpace(_csprojPath) && string.IsNullOrWhiteSpace(_csprojContent.Get()))
            throw new InvalidOperationException("Either csprojPath or csprojContent should be provided.");

        _csprojXdocument = XDocument.Load(new StringReader(_csprojContent.Get()));
    }

    public virtual bool HasProjectReference(string projectName)
    {
        var projectReferences = GetProjectReferenceElements();

        return projectReferences.Any(pr => pr.GetProjectName() == projectName);
    }

    public virtual string? GetProjectVersion()
        => _csprojXdocument.Descendants("Version").FirstOrDefault()?.Value;

    public virtual string GetProjectName()
    {
        if (string.IsNullOrWhiteSpace(_csprojPath))
            throw new InvalidOperationException("Csproj path is empty");

        return _csprojPath.Replace(".csproj", string.Empty).Split("\\").Last();
    }

    public virtual CsprojContent GetContent() => _csprojContent;


    private ProjectReferenceElement[] GetProjectReferenceElements()
    {
        var xElements = _csprojXdocument.Descendants("ProjectReference")?.ToArray() ?? [];

        if (xElements.Any() is false) return [];

        return xElements.Select(xe => ProjectReferenceElement.New().WithXElement(xe)).ToArray();
    }
}