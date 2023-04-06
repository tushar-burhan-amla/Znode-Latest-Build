using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchProfileViewModel : BaseViewModel
    {
        public int PublishCatalogId { get; set; }
        public int SearchProfileId { get; set; }
        public int? PortalId { get; set; }
        public int SearchQueryTypeId { get; set; }
        public int? SearchSubQueryTypeId { get; set; }

        public string QueryTypeName { get; set; }
        public string SubQueryName { get; set; }
        public string PortalName { get; set; }
        public int SearchFeatureId { get; set; }
        public int ParentSearchFeatureId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredSearchProfileName)]
        [Display(Name = ZnodeAdmin_Resources.TextSearchProfileName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorAttributeProfileName)]
        public string ProfileName { get; set; }
        public string SearchText { get; set; }
        public string Operator { get; set; } = "OR";
        public bool IsDefault { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCatalogRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelCatalogName, ResourceType = typeof(Admin_Resources))]
        public string CatalogName { get; set; }
        public List<SearchFeatureViewModel> FeaturesList { get; set; }
        public List<SearchQueryTypeViewModel> QueryTypes { get; set; }
        public List<SearchAttributesViewModel> SearchableAttributesList { get; set; }

        public List<KeyValuePair<string, int>> FieldValueFactors { get; set; }

        public List<FieldValueViewModel> FieldValueList { get; set; }
            
        public TabViewListModel Tabs { get; set; } = null;

        public bool IsIndexExist { get; set; }

        public List<SelectListItem> FieldList { get; set; }
        public string PublishStatus { get; set; }
        public string PublishStatusId { get; set; }
        public string CatalogCode { get; set; }
        public bool PublishRequired { get; set; }

    }
}
