using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CategoryListViewModel : BaseViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }
        public GridModel GridModel { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public string FileName { get; set; }
        public int SEOTypeId { get; set; }
        public int PimCatalogId { get; set; }        
        public int SEOId { get; set; }

        public Dictionary<string, object> AttrubuteColumnName { get; set; }
        public List<dynamic> XmlDataList { get; set; }

        public CategoryListViewModel()
        {
            Categories = new List<CategoryViewModel>();
        }

        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        public List<SelectListItem> PortalList { get; set; }
        public string CategoryIds { get; set; }
        public int PromotionId { get; set; }
        public List<SelectListItem> Locales { get; set; }
        public string PublishStatus { get; set; }
        public string SEOCode { get; set; }
        public string PimCatalogName { get; set; }
    }
}