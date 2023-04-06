using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceSKUDataViewModel : BaseViewModel
    {
        public PriceSKUViewModel PriceSKU { get; set; }
        public PriceTierViewModel PriceTier { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PricingListRequiredMessage)]
        public int? PriceListId { get; set; }
        public int? PriceId { get; set; }
        public GridModel GridModel { get; set; }
        public string ListName { get; set; }
        public List<SelectListItem> UOM { get; set; }
        public int TierPriceListId { get; set; }
        public string SKUTierPrice { get; set; }
        public List<PriceTierViewModel> PriceTierList { get; set; }
        public int ProductId { get; set; }
        public string PriceTierListData { get; set; }

        [FileTypeValidation(AdminConstants.CSVFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CSVFileTypeErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TemplateId, ResourceType = typeof(Admin_Resources))]
        public int TemplateId { get; set; }
        public List<BaseDropDownList> baseDropDownList { get; set; }
        public List<SelectListItem> TemplateTypeList { get; set; }
        public int ImportHeadId { get; set; }
        public string ImportType { get; set; }
        public int PageNumber { get; set; }
    }     
}