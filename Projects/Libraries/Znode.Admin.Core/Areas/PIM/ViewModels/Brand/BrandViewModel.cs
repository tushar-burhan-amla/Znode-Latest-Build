using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class BrandViewModel : BaseViewModel
    {
        public int BrandId { get; set; }
      
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodePIM_Resources.ErrorBrandCode, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string BrandCode { get; set; }
        public int? MediaId { get; set; }       
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string Description { get; set; }
        public string WebsiteLink { get; set; }
        public string SEOTitle { get; set; }
        public string SEOKeywords { get; set; }
        public string SEODescription { get; set; }

        [RegularExpression(AdminConstants.SEOUrlValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorValidSEOUrl)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SEOFriendlyPageName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodePIM_Resources.ErrorBrandName, ErrorMessageResourceType = typeof(PIM_Resources))]
        public string BrandName { get; set; }       
        public int DisplayOrder { get; set; } 
        public bool IsActive { get; set; } = true;
        public string MediaPath { get; set; } = "";
        public List<SelectListItem> BrandCodeList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public int CMSSEODetailId { get; set; }
        public int CMSSEODetailLocaleId { get; set; }
        public int BrandDetailLocaleId { get; set; }
        public int PromotionId { get; set; }
        public int? PortalId { get; set; }

        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int CMSWidgetBrandId { get; set; }


        public BrandViewModel()
        {
            BrandCodeList = new List<SelectListItem>();
        }
    }
}