using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class UrlRedirectListViewModel : BaseViewModel
    {
        public List<UrlRedirectViewModel> UrlRedirects { get; set; }
        public GridModel GridModel { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        public List<SelectListItem> PortalList { get; set; }
    }
}