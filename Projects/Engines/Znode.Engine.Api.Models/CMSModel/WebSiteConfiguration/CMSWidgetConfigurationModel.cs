using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CMSWidgetConfigurationModel : BaseModel
    {
        public int CMSWidgetSliderBannerId { get; set; }
        public int CMSWidgetsId { get; set; }
        public int CMSSliderId { get; set; }
        public int PortalId { get; set; }

        public string WidgetCode { get; set; }
        public string Type { get; set; }
        public string Navigation { get; set; }
        public bool AutoPlay { get; set; }
        public int? AutoplayTimeOut { get; set; }
        public bool AutoplayHoverPause { get; set; }
        public string TransactionStyle { get; set; }
        public int? CMSContentPagesId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int LocaleId { get; set; }
        public bool EnableCMSPreview { get; set; }

        public List<BannerModel> SliderBanners { get; set; }
    }
}
