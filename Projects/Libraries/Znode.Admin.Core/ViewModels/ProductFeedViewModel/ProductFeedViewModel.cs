using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductFeedViewModel : BaseViewModel
    {
        public ProductFeedModel ProductFeed { get; set; }
        public int ProductFeedId { get; set; }
        [Required]
        public int PortalId { get; set; }
        public string XMLSiteMap { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelXMLSiteMapType, ResourceType = typeof(Admin_Resources))]
        public string XMLSiteMapType { get; set; }
        public string SuccessXMLGenerationMessage { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelXMLFileName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredFileNameErrorMessage)]
        [RegularExpression(AdminConstants.FileNameRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RegularExpFileNameErrorMessage)]
        [StringLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.FileNameLengthError)]
        public string FileName { get; set; }
        public StoreListViewModel StoreList { get; set; }
        public List<SelectListItem> FrequencyList { get; set; }
        public List<SelectListItem> XMLSiteMapList { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelXMLSiteMap, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> XMLSiteMapTypeList { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelXMLSiteMap, ResourceType = typeof(Admin_Resources))]
        public string ErrorMessage { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelProductFeedTitle, ResourceType = typeof(Admin_Resources))]
        public string Title { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelProductFeedLink, ResourceType = typeof(Admin_Resources))]
        [RegularExpression("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|www\\.){1}([0-9A-Za-z-\\.@:%_+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ProductFeedLinkErrorMessage)]
        public string Link { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelProductFeedDescription, ResourceType = typeof(Admin_Resources))]
        public string Description { get; set; }
        public int LocaleId { get; set; }
        public List<SelectListItem> Locale { get; set; }
        public string DBDate { get; set; }
        public string DBTime { get; set; }
        public string ProductFeedTypeName { get; set; }
        public string ProductFeedTypeCode { get; set; }
        public string ProductFeedSiteMapTypeCode { get; set; }
        public string ConnectorTouchPoints { get; set; }
        public string SchedulerCallFor { get; set; }
        public string[] PortalIds { get; set; }
        public string StoreName { get; set; }
        public string LocaleName { get; set; }
    }
}