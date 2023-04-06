using System.Configuration;

namespace Znode.Engine.ERPConnector.Model
{
    public class XMLValidationModel : ConfigurationElement
    {
        [ConfigurationProperty("isrequired", IsKey = true)]
        public string IsRequired
        {
            get { return (string)this["isrequired"]; }
            set { this["isrequired"] = value; }
        }
        [ConfigurationProperty("regexpattern", IsKey = true)]
        public string RegexPattern
        {
            get { return (string)this["regexpattern"]; }
            set { this["regexpattern"] = value; }
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
