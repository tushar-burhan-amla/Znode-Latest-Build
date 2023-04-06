using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ProfileModel : BaseModel
    {
        public int AccountId { get; set; }
        public int AccountProfileId { get; set; }
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        [MaxLength(100)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.PleaseEnterProfileName)]
        public string ProfileName { get; set; }
        public int IsAssociated { get; set; }
        public decimal? Weighting { get; set; }
        public string DefaultExternalAccountNo { get; set; }
        public bool ShowOnPartnerSignup { get; set; }
        public bool TaxExempt { get; set; }
        public bool? IsDefault { get; set; }
        public string AccountProfileIds { get; set; }
        public string ProfileIds { get; set; }
        public int? ParentProfileId { get; set; }
        public int? PimCatalogId { get; set; }
        public int? PublishCatalogId { get; set; }
    }
}
