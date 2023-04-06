using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models.V2
{
    public class CreateUserModelV2 : BaseModel
    {
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
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z]).{8,}$")]
        [MinLength(6)]
        [MaxLength(128)]
        public string Password { get; set; }
        public bool IsSendPeriodicEmail { get; set; }
    }
}
