using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceUserViewModel : BaseViewModel
    {
        public int PriceListUserId { get; set; }
        public int PriceListId { get; set; }
        public int UserId { get; set; }
        [Range(1, 999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PrecedenceRangeError)]
        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PrecedenceRequired)]
        public int Precedence { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFullName, ResourceType = typeof(Admin_Resources))]
        public string Fullname { get; set; }
        public string AccountName { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
    }
}