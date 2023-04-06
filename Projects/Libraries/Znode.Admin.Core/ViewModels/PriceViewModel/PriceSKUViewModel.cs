using System;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceSKUViewModel : BaseViewModel
    {
        public int? PriceId { get; set; }
        public int? PriceListId { get; set; }
        public int PimProductId { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSKURange)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.SKURequiredMessage)]
        public string SKU { get; set; }

        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRetailPriceRange)]
        [Display(Name = ZnodeAdmin_Resources.LabelRetailPrice, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RetailPriceRequiredMessage)]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRetailPrice)]
        public decimal? RetailPrice { get; set; }

        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSalesPriceRange)]
        [Display(Name = ZnodeAdmin_Resources.LabelSalesPrice, ResourceType = typeof(Admin_Resources))]
        [Range(0,999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSalesPrice)]
        public decimal? SalesPrice { get; set; }

        [RegularExpression(AdminConstants.DecimalNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCostPrice)]
        [Display(Name = ZnodeAdmin_Resources.LabelCostPrice, ResourceType = typeof(Admin_Resources))]
        [Range(0, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCostPriceRange)]
        public decimal? CostPrice { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelActivationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExpirationDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? ExpirationDate { get; set; }

        public string ListName { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ExternalId { get; set; }

    }
}