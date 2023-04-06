using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class AssociatedParameterModel
    {
        public int PortalId { get; set; }

        /// <summary>
        /// This helps to pass in query parameter (Comma seperated string)
        /// </summary>
        [Required]
        public string AssociateIds { get; set; }

        [Required]
        public int PromotionId { get; set; }

        public string DiscountTypeName { get; set; }
    }
}
