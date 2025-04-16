using System.Xml.Linq;

namespace NuVerNet.DependencyResolver;

public class CsprojReader
{
    private string _csprojPath;
    private XDocument _csprojXdocument;

    private CsprojReader()
    {
    }

    public static CsprojReader New() => new();

    public CsprojReader WithCsprojPath(string csprojPath)
    {
        _csprojPath = csprojPath;
        return this;
    }

    public async Task LoadAsync()
    {
        var csprojContent = await File.ReadAllTextAsync(_csprojPath);
        _csprojXdocument = XDocument.Load(new StringReader(csprojContent));
    }

    public XElement[] GetProjectReferenceElements()
        => _csprojXdocument.Descendants("ProjectReference").ToArray();

    public string? GetProjectVersion()
        => _csprojXdocument.Descendants("Version").FirstOrDefault()?.Value;

    public string GetProjectName()
        => _csprojPath.Replace(".csproj", string.Empty).Split("\\").Last();
}