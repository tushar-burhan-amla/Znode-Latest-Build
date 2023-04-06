using System.ComponentModel.DataAnnotations;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Models
{
    public class GlobalMediaDisplaySettingModel : BaseModel
    {
        public int GlobalMediaDisplaySettingsId { get; set; }
        public int? MediaId { get; set; }
        [Required]
        public int MaxDisplayItems { get; set; }
        [Required]
        public int MaxSmallThumbnailWidth { get; set; }
        [Required]
        public int MaxSmallWidth { get; set; }
        [Required]
        public int MaxMediumWidth { get; set; }
        [Required]
        public int MaxThumbnailWidth { get; set; }
        [Required]
        public int MaxLargeWidth { get; set; }
        [Required]
        public int MaxCrossSellWidth { get; set; }

        public string MediaPath { get; set; }

        public string DefaultImageName { get; set; }

        public static GlobalMediaDisplaySettingModel GetGlobalMediaDisplaySetting()
        {
            return new GlobalMediaDisplaySettingModel
            {
                MaxSmallThumbnailWidth = ZnodeConstant.MaxSmallThumbnailWidth,
                MaxLargeWidth = ZnodeConstant.MaxLargeWidth,
                MaxSmallWidth = ZnodeConstant.MaxSmallWidth,
                MaxThumbnailWidth = ZnodeConstant.MaxThumbnailWidth,
                MaxMediumWidth = ZnodeConstant.MaxMediumWidth,
                MaxCrossSellWidth = ZnodeConstant.MaxCrossSellWidth,
            };
        }
    }
}
