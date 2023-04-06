using System.Configuration;

namespace Znode.Engine.ERPConnector.Model
{
    public class XMLControlModel : ConfigurationElement
    {

        [ConfigurationProperty("name", IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsKey = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("Id", IsKey = true)]
        public string Id
        {
            get { return (string)this["Id"]; }
            set { this["Id"] = value; }
        }

        [ConfigurationProperty("CSSClass", IsKey = true)]
        public string CSSClass
        {
            get { return (string)this["CSSClass"]; }
            set { this["CSSClass"] = value; }
        }

        [ConfigurationProperty("label", IsKey = true)]
        public string ControlLabel
        {
            get { return (string)this["label"]; }
            set { this["label"] = value; }
        }

        [ConfigurationProperty("type", IsKey = true)]
        public string ControlType
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
        [ConfigurationProperty("helptext", IsKey = true)]
        public string HelpText
        {
            get { return (string)this["helptext"]; }
            set { this["helptext"] = value; }
        }

        [ConfigurationProperty("isheader", IsKey = true)]
        public string IsHeader
        {
            get { return (string)this["isheader"]; }
            set { this["isheader"] = value; }
        }
        [ConfigurationProperty("validation", IsKey = true)]
        public XMLValidationModel Validations
        {
            get { return (XMLValidationModel)this["validation"]; }
            set { this["validation"] = value; }
        }
       
        [ConfigurationProperty("dropdownvalue", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(XMLSelectOption),
        AddItemName = "add")]
        public XMLSelectOptions DropDownValue
        {
            get
            {
                XMLSelectOptions options =
                    (XMLSelectOptions)base["dropdownvalue"];

                return options;
            }

            set
            {
                XMLSelectOptions options = value;
            }
        }

        [ConfigurationProperty("maxlength", IsKey = true)]
        public string MaxLength
        {
            get { return (string)this["maxlength"]; }
            set { this["maxlength"] = value; }
        }

        [ConfigurationProperty("oninput", IsKey = true)]
        public string OnInput
        {
            get { return (string)this["oninput"]; }
            set { this["oninput"] = value; }
        }
    }
}
