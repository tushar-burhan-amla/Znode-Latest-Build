using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomFieldViewModel : BaseViewModel
    {
        public int? ProductId { get; set; }
        [Display(Name = ZnodePIM_Resources.LabelCustomCode, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorCustomCodeRequired)]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(299, ErrorMessageResourceName = ZnodePIM_Resources.ErrorCustomCodeCharacterLength, ErrorMessageResourceType = typeof(PIM_Resources))]
        public string CustomCode { get; set; }
        public int CustomFieldId { get; set; }
        public string CustomKey { get; set; }
        public string CustomValue { get; set; }
        public List<CustomFieldLocaleModel> CustomFieldLocales { get; set; }
        public LocaleListViewModel Locales { get; set; }

        [Display(Name = ZnodeAttributes_Resources.LabelDisplayOrder, ResourceType = typeof(Attributes_Resources))]
        [Range(1, 999, ErrorMessageResourceName = ZnodeAttributes_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessageResourceName = ZnodeAttributes_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(Attributes_Resources))]
        public int? DisplayOrder { get; set; }
    }
}