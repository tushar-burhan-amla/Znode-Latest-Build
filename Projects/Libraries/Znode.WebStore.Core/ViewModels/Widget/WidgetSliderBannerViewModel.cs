using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetSliderBannerViewModel : BaseViewModel
    {
        public int WidgetSliderBannerId { get; set; }
        public int MappingId { get; set; }
        public int SliderId { get; set; }
        public int? AutoplayTimeOut { get; set; }

        public string Type { get; set; }
        public string Navigation { get; set; }
        public string TransactionStyle { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }

        public bool AutoPlay { get; set; }
        public bool AutoplayHoverPause { get; set; }

        public List<SliderBannerViewModel> SliderBanners { get; set; }
        
        public bool IsEmpty
        {
            get
            {
               return SliderBanners?.Count > 0 ? false : true;
              
            }
        }
    }
}