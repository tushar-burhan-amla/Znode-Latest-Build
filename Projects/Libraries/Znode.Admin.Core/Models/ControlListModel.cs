using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.Models
{
    /// <summary>
    /// Controls Descriptions list
    /// </summary>
    public class ControlListModel
    {
        public ControlListModel()
        {
            ControlDescriptions = new List<ControlDescription>();
        }
        public List<ControlDescription> ControlDescriptions { get; set; }
    }

    /// <summary>
    /// Validations
    /// </summary>
    public class Validations
    {
        public bool IsRequired { get; set; }
        public bool IsRegex { get; set; }
        public bool IsNumber { get; set; }
        public int? MaximumValue { get; set; }
        public int? MinimumValue { get; set; }
        public string RegexPattern { get; set; }
        public string ErrorMessage { get; set; }
        public int? MaxCharacters { get; set; }
        public string ValidationRule { get; set; }
        public string MaxDate { get; set; }
        public string MinDate { get; set; }
        public string AllowDecimals { get; set; }
        public string AllowNegative { get; set; }
        public int? MaxNumber { get; set; }
        public int? MinNumber { get; set; }
    }

    /// <summary>
    /// Properties details with validations
    /// </summary>
    public class Properties
    {
        public Properties()
        {
            validation = new List<Validations>();
            MultiSelectDropDownValues = new List<BaseDropDownList>();
        }
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string DefaultValue { get; set; }
        public string FileSize { get; set; }
        public int FileId { get; set; }
        public List<SelectListItem> DropDownValue { get; set; }
        public List<BaseDropDownList> MultiSelectDropDownValues { get; set; }
        public List<Validations> validation { get; set; }
        public bool ShowPropertyLabel { get; set; }
        public bool ShowMultiSelectSubmitButton{ get; set; }
        public DropDownOptions DropDownOptions { get; set; }
    }

    /// <summary>
    /// Properties List Model
    /// </summary>
    public class PropertiesListModel
    {
        public PropertiesListModel()
        {
            Properties = new List<Properties>();
        }
        public List<Properties> Properties { get; set; }
    }

    /// <summary>
    /// Control Description containing control type and properties list
    /// </summary>
    public class ControlDescription
    {
        public ControlDescription()
        {
            Properties = new PropertiesListModel();
        }
        public ControlType ControlType { get; set; }
        public PropertiesListModel Properties { get; set; }
    }

    /// <summary>
    ///BindDataModel is a dictionary  used to bind the form values in post 
    /// </summary>
    public class BindDataModel
    {
        public BindDataModel()
        {
            ControlsData = new Dictionary<string, object>();
        }
        public Dictionary<string, object> ControlsData { get; set; }

        public string message { get; set; }
    }

    /// <summary>
    /// Enum containing Control Type Names
    /// </summary>
    public enum ControlType
    {
        Date,
        File,
        Label,
        Text,
        MultiSelect,
        Number,
        YesNo,
        TextArea,
        SimpleSelect,
        Image
    }
}