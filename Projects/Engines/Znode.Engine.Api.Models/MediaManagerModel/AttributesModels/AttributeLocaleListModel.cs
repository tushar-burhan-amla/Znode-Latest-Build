using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesLocaleListModel:BaseListModel
    {
        public List<AttributesLocaleModel> Locales { get; set; }

        public string AttributeCode { get; set; }

        public AttributesLocaleListModel()
        {
            Locales = new List<AttributesLocaleModel>();
        }
    }
}
