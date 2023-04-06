using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class CustomerReviewModel : BaseModel
    {
        public int CMSCustomerReviewId { get; set; }
        public int? PublishProductId { get; set; }
        public int? UserId { get; set; }
        public int? Rating { get; set; }

        public string ProductName { get; set; }
        [Required]
        public string Headline { get; set; }
        [Required]
        public string Comments { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserLocation { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }

        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public string StoreName { get; set; }
        public int PortalId { get; set; }
        public string SKU { get; set; }
    }
}
