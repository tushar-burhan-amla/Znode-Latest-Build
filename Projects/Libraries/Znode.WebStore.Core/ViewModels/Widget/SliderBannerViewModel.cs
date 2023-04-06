using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class SliderBannerViewModel : BaseViewModel
    {
        public int SliderBannerId { get; set; }
        public int? BannerSequence { get; set; }

        public string MediaPath { get; set; }
        public string SliderBannerTitle { get; set; }
        public string ImageAlternateText { get; set; }
        public string ButtonLabelName { get; set; }
        public string ButtonLink { get; set; }
        public string TextAlignment { get; set; }
        public string Description { get; set; }

        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}