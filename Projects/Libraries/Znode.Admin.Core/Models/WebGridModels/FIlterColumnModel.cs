using System;
using System.Xml.Serialization;

namespace Znode.Engine.Admin.Models
{
    [Serializable]
    [XmlRoot("columns"), XmlType("columns")]
    public class FilterColumnModel
    {
        [XmlElement("name")]
        public string ColumnName { get; set; }

        [XmlElement("headertext")]
        public string HeaderText { get; set; }

        [XmlElement("datatype")]
        public string DataType { get; set; }

        public string Value { get; set; }

        public string SelectListOfDatatype { get; set; }

        public int DataOperatorId { get; set; }

        [XmlElement("isallowsearch")]
        public string IsAllowSearch { get; set; }

        [XmlElement("SearchControlType")]
        public string SearchControlType { get; set; }

        [XmlElement("SearchControlParameters")]
        public string SearchControlParameters { get; set; }
        public int Id { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsGlobalSearch { get; set; }
    }
}


