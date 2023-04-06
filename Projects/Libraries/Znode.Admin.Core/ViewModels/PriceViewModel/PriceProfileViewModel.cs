using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PriceProfileViewModel : BaseViewModel
    {
        public int PriceListProfileId { get; set; }
        public int PriceListId { get; set; }
        public int ProfileId { get; set; }
        public int Precedence { get; set; }
        public string StoreName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelProfileName, ResourceType = typeof(Admin_Resources))]
        public string ProfileName { get; set; }
    }
}