using System.Xml.Linq;

namespace NuVerNet.CentralPackageManagement;

public class CentralPackageManagement
{
    private XDocument _xDocument;
    private string _centralPackagePropsPath;
    private Dictionary<string, XElement> _packageVersionXElements = [];

    private CentralPackageManagement()
    {
    }

    public static CentralPackageManagement New() => new();

    public CentralPackageManagement WithCentralPackagePropsPath(string centralPackagePropsPath)
    {
        _centralPackagePropsPath = centralPackagePropsPath;
        return this;
    }

    public void Load()
    {
        _xDocument = XDocument.Load(_centralPackagePropsPath);
        _packageVersionXElements = _xDocument.Descendants("PackageVersion").ToDictionary(xe => xe.Attribute("Include")!.Value);
    }

    public void UpdatePackageVersion(string packageName, string version)
    {
        _packageVersionXElements.TryGetValue(packageName, out var packageVersionXElement);

        if (packageVersionXElement is null)
            return;

        ConsoleLog.Info($"Updating \"{packageName}\" version to \"{version}\" in the central package props");

        packageVersionXElement.SetAttributeValue("Version", version);
        File.WriteAllText(_centralPackagePropsPath, _xDocument.ToString());

        ConsoleLog.Success($"\"{packageName}\" version updated \"{version}\" successfully in the central package props");
    }
}