using System.Xml.Schema;
using System.Xml.Serialization;

namespace Znode.Engine.ERPConnector
{
    /// <summary>
    /// Model required for QuickBooks XML request attribute
    /// </summary>
    public class BaseModel
    {
        [XmlAttribute(AttributeName = "requestID", Form = XmlSchemaForm.Unqualified)]
        public string requestID { get; set; }
    }
}