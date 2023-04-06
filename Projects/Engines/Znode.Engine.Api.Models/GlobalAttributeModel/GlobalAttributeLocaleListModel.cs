using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeLocaleListModel : BaseListModel
    {
        public List<GlobalAttributeLocaleModel> Locales { get; set; }
        public string AttributeCode { get; set; }

        public GlobalAttributeLocaleListModel()
        {
            Locales = new List<GlobalAttributeLocaleModel>();
        }
    }
}
