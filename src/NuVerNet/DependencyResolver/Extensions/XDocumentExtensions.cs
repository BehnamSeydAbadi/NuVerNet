using System.Xml.Linq;

namespace NuVerNet.DependencyResolver.Extensions;

public static class XDocumentExtensions
{
    public static XElement[] GetProjectReferenceElements(this XDocument xDocument)
    {
        return xDocument.Descendants("ProjectReference").ToArray();
    }
}