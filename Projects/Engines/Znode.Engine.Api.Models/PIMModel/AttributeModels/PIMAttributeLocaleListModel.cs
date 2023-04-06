using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeLocaleListModel : BaseListModel
    {
        public List<PIMAttributeLocaleModel> Locales { get; set; }
        public string AttributeCode { get; set; }

        public PIMAttributeLocaleListModel()
        {
            Locales = new List<PIMAttributeLocaleModel>();
        }
    }
}
