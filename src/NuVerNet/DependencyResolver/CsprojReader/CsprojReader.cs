﻿using System.Xml.Linq;
using NuVerNet.DependencyResolver.CsprojReader.Exceptions;

namespace NuVerNet.DependencyResolver.CsprojReader;

public class CsprojReader
{
    private string _csprojPath;
    private string _csprojContent;

    private XDocument _csprojXdocument;

    private readonly Dictionary<string, XElement> _packageReferencesXElementsIndexedByPackageName = [];

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

    public CsprojReader WithCsprojContent(string csprojContent)
    {
        _csprojContent = csprojContent;
        return this;
    }

    public virtual void Load()
    {
        if (string.IsNullOrWhiteSpace(_csprojPath) && string.IsNullOrWhiteSpace(_csprojContent))
            throw new SourceNotFoundException();

        _csprojXdocument = GetCsprojXdocument();

        foreach (var xElement in _csprojXdocument.Descendants("PackageReference").ToArray())
        {
            _packageReferencesXElementsIndexedByPackageName.Add(xElement.Attribute("Include")!.Value, xElement);
        }
    }


    public virtual bool HasProjectReference(string projectName)
    {
        var projectReferences = GetProjectReferenceElements();

        return projectReferences.Any(pr => pr.GetProjectName() == projectName);
    }

    public virtual SemVersion? GetProjectVersion()
    {
        var xElement = GetProjectVersionXElement(_csprojXdocument);

        return xElement is null ? null : SemVersion.Parse(xElement.Value);
    }

    public virtual string GetProjectName()
    {
        if (string.IsNullOrWhiteSpace(_csprojPath))
            throw new CsprojPathIsEmptyException();

        return _csprojPath.Replace(".csproj", string.Empty).Split("\\").Last();
    }

    public ProjectModel GetProjectModel()
    {
        return new ProjectModel
        {
            Name = GetProjectName(),
            Path = GetRelativePath(),
            AbsolutePath = GetAbsolutePath(),
            CsprojContent = GetContent(),
            Version = GetProjectVersion()
        };
    }

    public virtual string GetContent() => _csprojContent;


    protected virtual string GetRelativePath() => _csprojPath;
    protected virtual string GetAbsolutePath() => _csprojPath;


    private ProjectReferenceElement[] GetProjectReferenceElements()
    {
        var xElements = _csprojXdocument.Descendants("ProjectReference")?.ToList() ?? [];
        xElements.AddRange(_csprojXdocument.Descendants("PackageReference")?.ToList() ?? []);

        if (xElements.Any() is false) return [];

        return xElements.Select(xe => ProjectReferenceElement.New().WithXElement(xe)).ToArray();
    }

    private XElement? GetProjectVersionXElement(XDocument xDocument)
        => xDocument.Descendants("Version").FirstOrDefault();

    private XDocument GetCsprojXdocument() => XDocument.Load(new StringReader(_csprojContent));

    public string[] GetPackageReferencesNames()
    {
        if (_packageReferencesXElementsIndexedByPackageName.Any() is false)
            return [];

        return _packageReferencesXElementsIndexedByPackageName.Select(kv => kv.Key).ToArray();
    }
}