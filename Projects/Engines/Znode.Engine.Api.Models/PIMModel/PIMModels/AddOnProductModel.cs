using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class AddOnProductModel : BaseModel
    {
        public int PimAddOnProductId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAddonGroupId { get; set; }
        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.RequiredDisplayOrder)]
        [Range(1, 999, ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorDisplayOrder)]
        public int? DisplayOrder { get; set; } = 500;
        public int? AddOnDisplayOrder { get; set; }
        public string RequiredType { get; set; }

        public AddonGroupModel ZnodePimAddonGroup { get; set; }
    }
}
