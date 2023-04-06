using System.Configuration;

namespace Znode.Engine.ERPConnector.Model
{
    public class XMLControlListModel : ConfigurationElementCollection
    {
        public XMLControlModel this[int index]
        {
            get
            {
                return base.BaseGet(index) as XMLControlModel;
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
            return new XMLControlModel();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((XMLControlModel)element).Name;
        }
    }
}
