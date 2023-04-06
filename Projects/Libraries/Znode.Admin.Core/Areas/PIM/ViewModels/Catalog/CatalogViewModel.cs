using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;
using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class CatalogViewModel : BaseViewModel
    {
        public int PimCatalogId { get; set; }

        [MaxLength(255)]
        [Display(Name = ZnodePIM_Resources.LabelCatalogName, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorCatalogRequired)]
        public string CatalogName { get; set; }
        public string UrlEncodedCatalogName { get; set; }
        public bool IsActive { get; set; }
        public int PortalId { get; set; }
        [Display(Name = ZnodePIM_Resources.LabelIsAllowIndexing, ResourceType = typeof(PIM_Resources))]
        public bool IsAllowIndexing { get; set; }
        [Display(Name = ZnodePIM_Resources.LabelDefaultStore, ResourceType = typeof(PIM_Resources))]
        public string DefaultStore { get; set; }
        public bool CopyAllData { get; set; }
        public string Status { get; set; }
        public string PreviewLink { get; set; } = "Preview";
        public List<SelectListItem> IsActiveList { get; set; }
        public string PublishStatus { get; set; }
        public DateTime? PublishCreatedDate { get; set; }
        public DateTime? PublishModifiedDate { get; set; }
        public int? PublishCategoryCount { get; set; }
        public int? PublishProductCount { get; set; }
        public string ConnectorTouchPoints { get; set; }
        public string SchedulerCallFor { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCatalogCode, ResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCatalogCodeRequired)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.CatalogCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string CatalogCode { get; set; }
    }
}