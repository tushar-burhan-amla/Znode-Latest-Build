using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeGroupLocaleListModel : BaseListModel
    {
        public List<PIMAttributeGroupLocaleModel> AttributeGroupLocales { get; set; }

        public string GroupCode { get; set; }

    }
}
