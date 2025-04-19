using System.Xml.Linq;

namespace NuVerNet.DependencyResolver.CsprojReader;

public class CsprojReader
{
    private string _csprojPath;
    private string _csprojContent;
    private XDocument _csprojXdocument;

    protected CsprojReader()
    {
    }

    public static CsprojReader New() => new();

    public CsprojReader WithCsprojPath(string csprojPath)
    {
        _csprojPath = csprojPath;
        _csprojContent = File.ReadAllText(csprojPath);
        return this;
    }

    public virtual void Load()
    {
        if (string.IsNullOrWhiteSpace(_csprojPath) && string.IsNullOrWhiteSpace(_csprojContent))
            throw new InvalidOperationException("Either csprojPath or csprojContent should be provided.");

        _csprojXdocument = XDocument.Load(new StringReader(_csprojContent));
    }

    public virtual bool HasProjectReference(string projectName)
    {
        var projectReferences = GetProjectReferenceElements();

        return projectReferences.Any(pr => pr.GetProjectName() == projectName);
    }

    public virtual SemVersion? GetProjectVersion()
    {
        var version = _csprojXdocument.Descendants("Version").FirstOrDefault()?.Value;
        return string.IsNullOrWhiteSpace(version) ? null : SemVersion.Parse(version);
    }

    public virtual string GetProjectName()
    {
        if (string.IsNullOrWhiteSpace(_csprojPath))
            throw new InvalidOperationException("Csproj path is empty");

        return _csprojPath.Replace(".csproj", string.Empty).Split("\\").Last();
    }

    public virtual string GetContent() => _csprojContent;


    private ProjectReferenceElement[] GetProjectReferenceElements()
    {
        var xElements = _csprojXdocument.Descendants("ProjectReference")?.ToArray() ?? [];

        if (xElements.Any() is false) return [];

        return xElements.Select(xe => ProjectReferenceElement.New().WithXElement(xe)).ToArray();
    }
}