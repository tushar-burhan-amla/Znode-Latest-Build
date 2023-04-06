﻿using System;
using System.Xml.Serialization;

namespace Znode.Libraries.DevExpress.Report
{
    [Serializable]
    [XmlRoot("columns"), XmlType("columns")]
    public class FilterModel
    {

        [XmlElement("name")]
        public string ColumnName { get; set; }

        [XmlElement("headertext")]
        public string HeaderText { get; set; }

        [XmlElement("datatype")]
        public string DataType { get; set; }

        [XmlElement("value")]
        public string Value { get; set; }

        [XmlElement("parametertype")]
        public string ParameterType { get; set; }

        public string SelectListOfDatatype { get; set; }

        public int DataOperatorId { get; set; }

        [XmlElement("isallowsearch")]
        public string IsAllowSearch { get; set; }

        [XmlElement("SearchControlType")]
        public string SearchControlType { get; set; }

        [XmlElement("SearchControlParameters")]
        public string SearchControlParameters { get; set; }

        [XmlElement("mustshow")]
        public string MustShow { get; set; }

        [XmlElement("musthide")]
        public string MustHide { get; set; }

        public int Id { get; set; }
        public bool IsSearchable { get; set; }


    }
}
