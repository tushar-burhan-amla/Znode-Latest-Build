using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PricePortalViewModel : BaseViewModel
    {
        public int PriceListPortalId { get; set; }
        public int PriceListProfileId { get; set; }
        public int PriceListId { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        public int PortalProfileId { get; set; }

        [Range(1, 999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PrecedenceRangeError)]
        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PrecedenceRequired)]
        public int Precedence { get; set; } 

        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public string StoreName { get; set; }
        public string CatalogName { get; set; }

        public string Name { get; set; }
    }
}