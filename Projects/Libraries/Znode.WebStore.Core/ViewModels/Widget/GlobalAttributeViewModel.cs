using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class GlobalAttributeViewModel : BaseViewModel
    {
        public int GlobalAttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeTypeName { get; set; }

        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; } = true;
        public bool IsLocalizable { get; set; } = true;
        public bool IsActive { get; set; }
        public int? AttributeGroupId { get; set; }
        public int? AttributeDisplayOrder { get; set; }
        public string HelpDescription { get; set; }
        public string AttributeType { get; set; }

        [Range(1, 999, ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        [RegularExpression("^([0-9][0-9]*)$", ErrorMessageResourceName = ZnodePIM_Resources.InvalidDisplayOrder, ErrorMessageResourceType = typeof(PIM_Resources))]
        public int? DisplayOrder { get; set; }
        public string AttributeDefaultValue { get; set; }
        public int? AttributeDefaultValueId { get; set; }
        public List<GlobalAttributeLocaleModel> Locales { get; set; }
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
        public GlobalAttributeViewModel()
        {
            Locales = new List<GlobalAttributeLocaleModel>();
        }
    }
}
