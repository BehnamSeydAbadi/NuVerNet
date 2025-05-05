using System.Xml.Linq;
using NuVerNet.DependencyResolver.CsprojWriter.Exceptions;

namespace NuVerNet.DependencyResolver.CsprojWriter;

public class CsprojWriter
{
    private string _version;
    private PackageReferenceVersion[] _packageReferenceVersions;
    private XDocument _csprojXdocument;
    private string _name;

    protected CsprojWriter()
    {
    }

    public static CsprojWriter New() => new();

    public CsprojWriter WithVersion(string version)
    {
        _version = version;
        return this;
    }

    public CsprojWriter WithPackageReferenceVersions(params PackageReferenceVersion[] packageReferenceVersions)
    {
        _packageReferenceVersions = packageReferenceVersions;
        return this;
    }

    public CsprojWriter WithCsprojContent(string csprojContent)
    {
        _csprojXdocument = XDocument.Load(new StringReader(csprojContent));
        return this;
    }

    public CsprojWriter WithProjectName(string name)
    {
        _name = name;
        return this;
    }

    public void Write(string csprojPath)
    {
        var csprojContent = GetBumpedVersionCsprojContent();

        csprojContent = GetPackageReferenceVersionUpdatedCsprojContent(csprojContent);

        File.WriteAllText(csprojPath, csprojContent);
    }

    private string GetBumpedVersionCsprojContent()
    {
        var versionXelement = _csprojXdocument.Descendants("Version").FirstOrDefault();

        if (versionXelement is null) 
            return _csprojXdocument.ToString();

        versionXelement.SetValue(_version);

        return versionXelement.Document!.ToString();
    }

    private string GetPackageReferenceVersionUpdatedCsprojContent(string csprojContent)
    {
        var csprojContentXDocument = XDocument.Load(new StringReader(csprojContent));

        var packageReferencesIndexedByName = csprojContentXDocument.Descendants("PackageReference")
            .ToDictionary(xe => xe.Attribute("Include")!.Value);

        foreach (var packageReferenceVersion in _packageReferenceVersions)
        {
            var packageReferenceXElement = packageReferencesIndexedByName[packageReferenceVersion.Name];

            if (packageReferenceXElement.Attribute("Version") is not null)
            {
                packageReferenceXElement.SetAttributeValue("Version", packageReferenceVersion.Version);
            }
        }

        return csprojContentXDocument.ToString();
    }
}