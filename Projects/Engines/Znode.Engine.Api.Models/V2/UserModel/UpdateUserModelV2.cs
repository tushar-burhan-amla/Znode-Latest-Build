using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class UpdateUserModelV2 : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int PortalId { get; set; }
        public int AccountId { get; set; }
        public int ProfileId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public bool IsSendPeriodicEmail { get; set; }
    }
}
