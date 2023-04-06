using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductDetailsListViewModel : BaseViewModel
    {
        public List<ProductDetailsViewModel> ProductDetailList { get; set; }
        public GridModel GridModel { get; set; }
        public List<dynamic> ProductDetailListDynamic { get; set; }
        public List<dynamic> NewAttributeList { get; set; }

        public Dictionary<string, object> AttrubuteColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }

        public ProductDetailsListViewModel()
        {
            ProductDetailList = new List<ProductDetailsViewModel>();
        }

        public int ParentProductId { get; set; }
        public string AssociatedConfigureAttributeIds { get; set; }
        public string AssociatedProductIds { get; set; }
        public int AttributeId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOfMapping { get; set; }
        public int CMSWidgetProductId { get; set;}
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public string FileName { get; set; }
        public int ListType { get; set; }
        public int AddonProductId { get; set; }
        public int SEOTypeId { get; set; }
        public int SEOId { get; set; }
        public int PimCatalogId { get; set; }
        public int PimCategoryId { get; set; }
        public int PimCategoryHierarchyId { get; set; }
        public int HighlightId { get; set; }
        public int AddonGroupId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        public List<SelectListItem> PortalList { get; set; }
        public int? ProfileCatalogId { get; set; }
        public int? ProfileId { get; set; }
        public int LocaleId { get; set; }
        public string PublishStatus {get;set;}
        public List<SelectListItem> Locales { get; set; }
        public int? DisplayOrder { get; set; }
        public string PimCatalogName { get; set; }
        public string ProductType { get; set; }
    }
}