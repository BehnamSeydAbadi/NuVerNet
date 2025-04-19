using System.Xml.Linq;

namespace NuVerNet.DependencyResolver;

public class CsprojContent
{
    private readonly string _content;
    private readonly string? _version;
    private string? _bumpedVersion;

    public CsprojContent(string content)
    {
        _content = content;
        var xDocument = XDocument.Load(new StringReader(content));
        _version = xDocument.Descendants("Version").FirstOrDefault()?.Value;
    }

    public string? GetVersion() => _version;

    public string? GetBumpedVersion() => _bumpedVersion;

    public string Get() => _content;

    public override string ToString() => _content;

    public override bool Equals(object? obj)
    {
        var that = (obj as CsprojContent)!;
        return that._content == this._content;
    }

    public override int GetHashCode() => _content.GetHashCode();

    public void BumpVersion()
    {
        if (string.IsNullOrWhiteSpace(_bumpedVersion) is false)
            throw new InvalidOperationException("Cannot bump version after it has been bumped.");

        _bumpedVersion = SemVersion.Parse(_version!).IncreasePatch().ToString();
    }
}