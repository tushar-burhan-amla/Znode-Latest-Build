using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class RemoveCartItemModelV2 : BaseModel
    {
        [Required]
        public string CookieMappingId { get; set; }
        [Required]
        public string SavedCartLineItemIds { get; set; }
         
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}")]
        public int PortalId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}")]
        public int PublishedCatalogId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}")]
        public int LocaleId { get; set; }
        [Required]        
        public int ProfileId { get; set; }
    }
}
