using System;

namespace Znode.Engine.Api.Models
{
    public class BannerModel : BaseModel
    {
        public int CMSSliderBannerId { get; set; }
        public int CMSSliderId { get; set; }
        public int? MediaId { get; set; }
        public string Title { get; set; }
        public string ImageAlternateText { get; set; }
        public string ButtonLabelName { get; set; }
        public string ButtonLink { get; set; }
        public string TextAlignment { get; set; }
        public int? BannerSequence { get; set; }
        public string Description { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string MediaPath { get; set; }
        public string Name { get; set; }
        public int LocaleId { get; set; }
        public string FileName { get; set; }
    }
}
