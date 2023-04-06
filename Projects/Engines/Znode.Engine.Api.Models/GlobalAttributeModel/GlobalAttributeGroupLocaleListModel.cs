using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeGroupLocaleListModel : BaseListModel
    {
        public string GroupCode { get; set; }
        public List<GlobalAttributeGroupLocaleModel> AttributeGroupLocales { get; set; }        
    }
}
