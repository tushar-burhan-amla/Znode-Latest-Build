using System.Xml.Schema;
using System.Xml.Serialization;

namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Model required for QuickBook XML request attribute
    /// </summary>
    public class BaseModel
    {
        [XmlAttribute(AttributeName = "requestID", Form = XmlSchemaForm.Unqualified)]
        public string requestID { get; set; }
    }
}