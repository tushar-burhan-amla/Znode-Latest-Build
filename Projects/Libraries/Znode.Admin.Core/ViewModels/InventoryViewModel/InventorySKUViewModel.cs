using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class InventorySKUViewModel : BaseViewModel
    {
        [MaxLength(300, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSKURange)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.SKURequiredMessage)]
        public string SKU { get; set; }

        [RegularExpression(AdminConstants.DecimalPositiveNegativeNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorQuantityOnHand)]
        [Display(Name = ZnodeAdmin_Resources.LabelQuantity, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.QuantityRequiredMessage)]
        [Range(typeof(decimal), "-999999", "999999", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorQuantity)]
        public decimal? Quantity { get; set; }

        [RegularExpression(AdminConstants.DecimalPositiveNegativeNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorReOrderLevel)]
        [Display(Name = ZnodeAdmin_Resources.LabelReOrderLevel, ResourceType = typeof(Admin_Resources))]
        [Range(typeof(decimal), "-999999", "999999", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorReOrderRange)]
        public decimal? ReOrderLevel { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBackOrderQuantity, ResourceType = typeof(Admin_Resources))]
        [Range(typeof(decimal), "0", "999999", ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorReOrderRange)]
        public decimal? BackOrderQuantity { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBackOrderExpectedDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? BackOrderExpectedDate { get; set; }

        public string ListName { get; set; }
        public string ListCode { get; set; }

        public int InventoryId { get; set; }
        public int? InventoryListId { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSelectWarehouse, ErrorMessageResourceType = typeof(Admin_Resources))]
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseDisplayName { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public List<SelectListItem> WarehouseNameList { get; set; }

        [FileTypeValidation(AdminConstants.CSVFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CSVFileTypeErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TemplateId, ResourceType = typeof(Admin_Resources))]
        public int TemplateId { get; set; }

        public List<SelectListItem> TemplateTypeList { get; set; }

        public List<DownloadableProductKeyViewModel> DownloadableProductKeys { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelImportType, ResourceType = typeof(Admin_Resources))]
        public int ImportHeadId { get; set; }
        public string ImportType { get; set; }
        public string ExternalId { get; set; }
        public bool IsDownloadable { get; set; }
        public int PimProductId { get; set; }
        public bool IsFromWarehouse { get; set; }
        public string ModifiedByName { get; set; }
    }
}