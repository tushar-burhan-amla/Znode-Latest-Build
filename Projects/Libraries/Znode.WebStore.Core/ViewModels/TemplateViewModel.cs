using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class TemplateViewModel : BaseViewModel
    {
        [StringLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ErrorTemplateNameLength)]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidTemplateName)]
        [Display(Name = ZnodeWebStore_Resources.TextTemplateName, ResourceType = typeof(WebStore_Resources))]
        public string TemplateName { get; set; }
        public int UserId { get; set; }
        public List<TemplateCartItemViewModel> TemplateCartItems { get; set; }
        public int OmsTemplateId { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int PublishedCatalogId { get; set; }
        public string OmsTemplateLineItemId { get; set; }
        public string CurrencyCode { get; set; }
        public int? Items { get; set; }
        public bool IsQuickOrderPad { get; set; }
        public bool IsViewMode { get; set; }
        public string CultureCode { get; set; }
        // IsAddToCartButtonDisable is used to disable Add to cart button on template page
        public bool IsAddToCartButtonDisable { get; set; }
        public string TemplateType { get; set; }
    }
}