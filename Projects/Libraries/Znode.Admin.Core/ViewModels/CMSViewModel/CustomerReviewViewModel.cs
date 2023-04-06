using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomerReviewViewModel : BaseViewModel
    {
        public int CMSCustomerReviewId { get; set; }
        public int? PublishProductId { get; set; }
        public int? UserId { get; set; }
        public int? Rating { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelProductName, ResourceType = typeof(Admin_Resources))]
        public string ProductName { get; set; }

        [MaxLength(200, ErrorMessageResourceName = ZnodeAdmin_Resources.HeadlineMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredHeadlineErrorMessage)]
        public string Headline { get; set; }

        [MaxLength(500, ErrorMessageResourceName = ZnodeAdmin_Resources.CommentsMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredCommentErrorMessage)]
        public string Comments { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.UserNameMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.Username, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredUserNameErrorMessage)]
        public string UserName { get; set; }

        [MaxLength(200, ErrorMessageResourceName = ZnodeAdmin_Resources.UserLocationMaxLengthMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelUserLocation, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredUserLocationErrorMessage)]
        public string UserLocation { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelReviewStatus, ResourceType = typeof(Admin_Resources))]
        public string Status { get; set; }

        public string StoreName { get; set; }

        public List<SelectListItem> GetReviewStatus { get; set; }

        public List<SelectListItem> GetReviewRatings { get; set; }

        public string SKU { get; set; }
    }
}