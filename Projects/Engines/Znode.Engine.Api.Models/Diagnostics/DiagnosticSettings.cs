using System.Collections.Generic;
using System.Xml.Serialization;

namespace Znode.Engine.Api.Models
{

    [XmlRoot(ElementName = "Diagnostic")]
    public class Diagnostic
    {
        [XmlElement(ElementName = "Category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "IsVisible", DataType = "boolean")]
        public bool IsVisible { get; set; }
        [XmlElement(ElementName = "SortOrder", DataType = "double")]
        public double SortOrder { get; set; }
        [XmlElement(ElementName = "Type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "DiagnosticSettings")]
    public class DiagnosticSettings
    {
        [XmlElement(ElementName = "Diagnostic")]
        public List<Diagnostic> Diagnostic { get; set; }
    }

}
