using System.Configuration;
namespace Znode.Engine.ERPConnector.Model
{
    public class XMLConfigurationModel : ConfigurationSection
    {
       
        [ConfigurationProperty("controls")]
        public XMLControlListModel Controls
        {
            get
            {
                return this["controls"] as XMLControlListModel;
            }
        }
       
    }
}
