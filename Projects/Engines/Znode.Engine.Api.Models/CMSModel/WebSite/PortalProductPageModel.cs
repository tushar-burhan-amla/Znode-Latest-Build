using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalProductPageModel : BaseModel
    {
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public string TemplateName { get; set; }
        public string Templates { get; set; }
        public Dictionary<int, string> ProductTypes { get; set; }
        public string ProductType { get; set; }
        public List<PortalProductPageModel> PortalProductPageList { get; set; }
        public List<string> TemplateNameList { get; set; }
        public List<string> ProductTypeList { get; set; }
    }
}
