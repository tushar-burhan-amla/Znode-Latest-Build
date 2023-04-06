using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceAccountViewModel : BaseViewModel
    {
        public int PriceListAccountId { get; set; }
        public int PriceListId { get; set; }
        public int AccountId { get; set; }

        [Range(1, 999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PrecedenceRangeError)]
        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PrecedenceRequired)]
        public int Precedence { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccountName, ResourceType = typeof(Admin_Resources))]
        public string AccountName { get; set; }
        public string PriceListIds { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public string ParentAccountName { get; set; }
        public string AccountCode { get; set; }
    }
}
