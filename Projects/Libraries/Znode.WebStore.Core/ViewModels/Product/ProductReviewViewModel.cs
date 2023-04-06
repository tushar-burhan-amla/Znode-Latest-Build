using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductReviewViewModel : BaseViewModel
    {
        public int? UserId { get; set; }
        public int PublishProductId { get; set; }
        [Range(1, 5, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredRating)]
        public decimal Rating { get; set; }
        public int CMSCustomerReviewId { get; set; }

        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredComments)]
        [MaxLength(500, ErrorMessageResourceName = ZnodeWebStore_Resources.CutomerReviewLength, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string Comments { get; set; }


        [MaxLength(300, ErrorMessageResourceName = ZnodeWebStore_Resources.CustomerUsernameLength, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredReviewName)]
        public string UserName { get; set; }

        public string Status { get; set; }

        [MaxLength(200, ErrorMessageResourceName = ZnodeWebStore_Resources.HeadlineLength, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredReviewHeadline)]
        public string Headline { get; set; }

        [MaxLength(200, ErrorMessageResourceName = ZnodeWebStore_Resources.UserLocationLength, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.RequiredReviewLocation)]
        public string UserLocation { get; set; }

        public string Duration { get; set; }
        public string ProductName { get; set; }

        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }

        public int PortalId { get; set; }
        public string SKU { get; set; }

    }
}