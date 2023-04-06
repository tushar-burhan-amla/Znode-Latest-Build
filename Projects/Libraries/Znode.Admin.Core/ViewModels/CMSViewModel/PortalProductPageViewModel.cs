using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalProductPageViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public string ProductType { get; set; }
        public string PortalName { get; set; }
        public string TemplateName { get; set; }
        public string Templates { get; set; }
        public List<SelectListItem> TemplateNames { get; set; }
        public Dictionary<string, string> ProductTypes { get; set; }
        public List<string> TemplateNameList { get; set; }
        public List<string> ProductTypeList { get; set; }
        public List<PortalProductPageViewModel> PortalProductPageList { get; set; }
    }
}