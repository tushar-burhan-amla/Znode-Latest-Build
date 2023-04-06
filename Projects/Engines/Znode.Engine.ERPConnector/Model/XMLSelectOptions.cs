using System.Configuration;

namespace Znode.Engine.ERPConnector.Model
{
    public class XMLSelectOptions : ConfigurationElementCollection
    {
        public XMLSelectOption this[int index]
        {
            get
            {
                return base.BaseGet(index) as XMLSelectOption;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new XMLSelectOption();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((XMLSelectOption)element).ItemText;
        }
    }


    public class XMLSelectOption : ConfigurationElement
    {
        [ConfigurationProperty("text", IsKey = true)]
        public string ItemText
        {
            get { return (string)this["text"]; }
            set { this["text"] = value; }
        }

        [ConfigurationProperty("value", IsKey = true)]
        public string ItemValue
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

    }

}
