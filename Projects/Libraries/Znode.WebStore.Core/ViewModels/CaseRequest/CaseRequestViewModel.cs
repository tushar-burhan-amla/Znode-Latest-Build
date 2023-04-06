using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CaseRequestViewModel : BaseViewModel
    {
        public int CaseRequestID { get; set; }
        public int PortalId { get; set; }
        public int? UserId { get; set; }
        public int CaseStatusId { get; set; }
        public int CasePriorityId { get; set; }
        public int CaseTypeId { get; set; }
        public string CaseOrigin { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelComments, ResourceType = typeof(WebStore_Resources))]
        public string Description { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessageResourceName = ZnodeWebStore_Resources.ErrorSpecialCharatersNotAllowed, ErrorMessageResourceType = typeof(WebStore_Resources))]

        public string FirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessageResourceName = ZnodeWebStore_Resources.ErrorSpecialCharatersNotAllowed, ErrorMessageResourceType = typeof(WebStore_Resources))]

        public string LastName { get; set; }
        public string CompanyName { get; set; }

        public string EmailID { get; set; }
        public string PhoneNumber { get; set; }

        public bool AllowSharingWithCustomer { get; set; }
        public string Message { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public int LocaleId { get; set; }
    }
}