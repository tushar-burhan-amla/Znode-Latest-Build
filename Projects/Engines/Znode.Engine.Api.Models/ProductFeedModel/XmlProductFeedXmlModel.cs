using System.Collections.Generic;
using System.Xml.Serialization;

namespace Znode.Engine.Api.Models
{
    [XmlRoot(ElementName = "channel")]
    public class XmlProductFeedXmlModel
    {
        [XmlElement(ElementName = "title")]
        public string Title;
        [XmlElement(ElementName = "link")]
        public string Link;
        [XmlElement(ElementName = "description")]
        public string Description;
        [XmlElement(ElementName = "item")]
        public List<XmlProductFeedModel> products;
    }
}
