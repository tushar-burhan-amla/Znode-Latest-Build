using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class AccountDepartmentModel : BaseModel
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorDepartmentNameRequired)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorDepartmentNameRange)]
        [Display(Name = ZnodeAdmin_Resources.LabelDepartmentName, ResourceType = typeof(Admin_Resources))]
        public string DepartmentName { get; set; }

        public int AccountId { get; set; }
    }
}
