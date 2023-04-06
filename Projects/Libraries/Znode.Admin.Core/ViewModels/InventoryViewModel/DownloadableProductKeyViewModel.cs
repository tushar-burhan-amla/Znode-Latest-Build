using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class DownloadableProductKeyViewModel : BaseViewModel
    {
        public int? ProductId { get; set; }
        public int PimDownloadableProductKeyId { get; set; }
        public string SKU { get; set; }
        public int rowIndexId { get; set; }

        [Display(Name = ZnodePIM_Resources.LabelProductKey, ResourceType = typeof(PIM_Resources))]
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorProductKeyRequired)]
        [MaxLength(1000, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorProductKeyRange)]
        public string DownloadableProductKey { get; set; }

        [RegularExpression(AdminConstants.DownloadableProductURLValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidationUrl)]
        [MaxLength(200, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorProductURLRange)]
        public string DownloadableProductURL { get; set; }

        public bool IsUsed { get; set; }
        public bool IsDuplicate { get; set; }
        public List<DownloadableProductKeyViewModel> DownloadableProductKeyList { get; set; }
    }
}
