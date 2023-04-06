using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchAttributesViewModel : BaseViewModel
    {
        public string AttributeName { get; set; }
        public string AttributeCode { get; set; }
        [Range(typeof(int), "1", "999")]
        [Display(Name ="Boost Value")]
        [RegularExpression(AdminConstants.WholeNoRegularExpression, ErrorMessage = "The field Boost Value must be a number.")]
        public int? BoostValue { get; set; } = 1;
        public bool IsFacets { get; set; }
        public bool IsUseInSearch { get; set; }
        public int SearchProfileId { get; set; }
        public int SearchProfileAttributeMappingId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsNgramEnabled { get; set; }
    }
}
