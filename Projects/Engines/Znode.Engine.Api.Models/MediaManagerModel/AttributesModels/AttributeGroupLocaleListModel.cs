using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributeGroupLocaleListModel : BaseListModel
    {
        public List<AttributeGroupLocaleModel> AttributeGroupLocales { get; set; }

        public string GroupCode { get; set; }

        public AttributeGroupLocaleListModel()
        {
            AttributeGroupLocales = new List<AttributeGroupLocaleModel>();
        }
    }
}
