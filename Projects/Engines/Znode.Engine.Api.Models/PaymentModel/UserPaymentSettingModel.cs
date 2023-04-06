using System.ComponentModel.DataAnnotations;
namespace Znode.Engine.Api.Models
{
    public class UserPaymentSettingModel : BaseModel
    {
        [Required(ErrorMessage = "This field is required")]
        public int PortalId { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public int UserId { get; set; }
    }
}
