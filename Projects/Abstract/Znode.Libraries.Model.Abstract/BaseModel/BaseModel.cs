using System;
using System.Xml.Serialization;
namespace Znode.Libraries.Abstract.Models
{
    public abstract class BaseModel
    {
        [XmlIgnore]
        public int CreatedBy { get; set; }
        [XmlIgnore]
        public DateTime CreatedDate { get; set; }
        [XmlIgnore]
        public int ModifiedBy { get; set; }
        [XmlIgnore]
        public DateTime ModifiedDate { get; set; }
        public string ActionMode { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
    }
}
