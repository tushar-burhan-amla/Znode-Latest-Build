using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContentPageListViewModel : BaseViewModel
    {
        public List<ContentPageViewModel> ContentPageList { get; set; }
        public GridModel GridModel { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public int SEOTypeId { get; set; }
        public string Tree { get; set; }
        public int SEOId { get; set; }

        public string SEOCode { get; set; }

        public string SEOPublishStatus { get; set; }

        public string PublishStatus { get; set; }
        public int FolderId { get; set; }
        public bool IsRootFolder { get; set; }
        public List<SelectListItem> PortalList { get; set; }

        public PopupViewModel PopupViewModel { get; set; }

        public bool IsPublished { get; set; }
        public List<SelectListItem> Locales { get; set; }
    }

}
