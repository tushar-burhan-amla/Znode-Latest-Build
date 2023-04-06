using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContainerVariantViewModel : BaseViewModel
    {
        public ContainerVariantViewModel()
        {
            Profiles = new List<SelectListItem>();
        }

        public List<SelectListItem> Profiles { get; set; }       
        public List<SelectListItem> Portals { get; set; }

        public int? ProfileId { get; set; }
        public int? PortalId { get; set; }
        public int ContentContainerId { get; set; }       
        public string ProfileName { get; set; }
        public string ProfileCode { get; set; }
        public string StoreName { get; set; }
        public string StoreCode{ get; set; }
        public string ContainerKey { get; set; }
        public int ContainerProfileVariantId { get; set; }
        public int? ContainerTemplateId { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public bool IsActive { get; set; } //This property is added in order to contain the active/deactive status of variants
        public string PublishStatus { get; set; } //This property is added in order to contain the publish status of variants
        public bool Status { get; set; } //This property is added in order to set the enable/disable icon on the variants list.
    }


}

