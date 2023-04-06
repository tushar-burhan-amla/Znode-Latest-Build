using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreAddOnModel : BaseModel
    {
        public string GroupName { get; set; }
        public string DisplayType { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
        public bool IsAutoAddon { get; set; }
        public string AutoAddonSKUs { get; set; }
        public List<WebStoreAddOnValueModel> AddOnValues { get; set; }
    }
}
