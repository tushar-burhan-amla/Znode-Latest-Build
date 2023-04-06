using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class WebStoreCaseRequestModel : BaseModel
    {
        public int CaseRequestId { get; set; }
        public int PortalId { get; set; }
        public int CaseStatusId { get; set; }
        public int CasePriorityId { get; set; }
        public int CaseTypeId { get; set; }
        public int? UserId { get; set; }

        public string CaseOrigin { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        [Required]
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string CasePriorityName { get; set; }
        public string CaseStatusName { get; set; }
        public string CaseTypeName { get; set; }
        public string EmailMessage { get; set; }
        public string StoreName { get; set; }
        public string EmailSubject { get; set; }
        public string FullName { get; set; }
        public string AttachedPath { get; set; }
        public int? CaseRequestHistoryId { get; set; }
        public string UserName { get; set; }
        public int LocaleId { get; set; }
    }
}
